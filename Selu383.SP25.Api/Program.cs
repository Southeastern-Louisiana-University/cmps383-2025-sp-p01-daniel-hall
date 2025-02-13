using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.Api.Entity;
using IdentityModel;
using Selu383.SP25.Api.Data.Services;
using Selu383.SP25.Api.Entity.Payment;
using Stripe;

namespace Selu383.SP25.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Stripe
            var stripeSettings = builder.Configuration.GetSection("Stripe");
            builder.Services.Configure<StripeSettings>(stripeSettings);
            StripeConfiguration.ApiKey = stripeSettings["SecretKey"];

            // Enable CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Database connection
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));

            // Identity configuration
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            // Register application services
            builder.Services.AddScoped<IMoviesService, MoviesService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<ITheatersService, TheatersService>();
            builder.Services.AddScoped<UserManager<User>>();

            var app = builder.Build();

            // Configure middleware
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));
            app.UseHttpsRedirection();

            // Apply CORS policy before authentication
            app.UseCors("AllowAll");
            app.UseAuthorization();

            // Database migration and seeding
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.Database.MigrateAsync();
                await TheaterSeeder.Initialize(dbContext);
            }

            app.MapControllers();
            app.Run();
        }
    }
}
