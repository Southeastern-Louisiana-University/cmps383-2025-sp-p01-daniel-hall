using Selu383.SP25.Api;
using Selu383.SP25.Api.Entity;

public class TheaterSeeder
{
    public static async Task Initialize(DataContext context)
    {
        if (!context.Theaters.Any())
        {
            await SeedTheaters(context);
        }
    }

    private static async Task SeedTheaters(DataContext context)
    {
        var seededTheaters = new List<Theater>()
        {
            new Theater
            {
                Name = "Grand Cinemas",
                SeatCount = 200,
                Address = "123 Movie Blvd, City Center"
            },
            new Theater
            {
                Name = "Luxury Theaters",
                SeatCount = 150,
                Address = "456 Luxe Ave, Uptown"
            },
            new Theater
            {
                Name = "Downtown Cinema",
                SeatCount = 300,
                Address = "789 Main Street, Downtown"
            }
        };

        context.Theaters.AddRange(seededTheaters);
        await context.SaveChangesAsync();
    }
}