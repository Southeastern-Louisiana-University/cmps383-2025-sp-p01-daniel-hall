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

            // Add services to the container.
            builder.Services.AddControllers();

            // Swagger implementation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // SQL Connection
            builder.Services.AddDbContext<DataContext>(Options =>
            {
                Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Dependency injection for user manager
            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject; // something for claims
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            // Service Injections
            builder.Services.AddScoped<IMoviesService, MoviesService>();
            builder.Services.AddScoped<IReviewsService, ReviewsService>();
            builder.Services.AddScoped<ITheatersService, TheatersService>();

            // Register UserManager
            builder.Services.AddScoped<UserManager<User>>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Redirect root URL to Swagger UI
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", "swagger"));

            // Ensure database is deleted and recreated
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();


                //await dbContext.Database.EnsureDeletedAsync();
                //await dbContext.Database.EnsureCreatedAsync();
                await dbContext.Database.MigrateAsync();


                await TheaterSeeder.Initialize(dbContext);
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
