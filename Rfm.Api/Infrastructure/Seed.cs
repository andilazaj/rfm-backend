using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;

namespace Rfm.Api;

public class Seed
{
    public async Task Run(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        // Create roles
        foreach (var roleName in new[] { "Admin", "Operator" })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleName
                });
            }
        }

        // Create admin user
        var adminEmail = "admin@demo.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            await userManager.CreateAsync(adminUser, "Admin!123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Create operator user
        var opEmail = "op@demo.com";
        var opUser = await userManager.FindByEmailAsync(opEmail);
        if (opUser == null)
        {
            opUser = new AppUser { UserName = opEmail, Email = opEmail, EmailConfirmed = true };
            await userManager.CreateAsync(opUser, "Op!12345");
            await userManager.AddToRoleAsync(opUser, "Operator");
        }

        // Only seed base data if not already seeded
        if (!await db.TourOperators.AnyAsync())
        {
            db.TourOperators.AddRange(
                new TourOperator { Name = "Aurora Travel" },
                new TourOperator { Name = "SkyLine" }
            );

            db.Routes.AddRange(
                new FlightRoute { Origin = "Tirana (TIA)", Destination = "Frankfurt (FRA)" },
                new FlightRoute { Origin = "Tirana (TIA)", Destination = "Munich (MUC)" }
            );

            db.Seasons.AddRange(
     new Season
     {
         Name = "Summer 2025",
         Start = new DateOnly(2025, 6, 1).ToDateTime(TimeOnly.MinValue),
         End = new DateOnly(2025, 9, 30).ToDateTime(TimeOnly.MinValue),
         Year = 2025
     },
     new Season
     {
         Name = "Winter 2025",
         Start = new DateOnly(2025, 12, 1).ToDateTime(TimeOnly.MinValue),
         End = new DateOnly(2026, 3, 31).ToDateTime(TimeOnly.MinValue),
         Year = 2025
     }
 );
            db.BookingClasses.AddRange(
                new BookingClass { Name = "Economy" },
                new BookingClass { Name = "Business" }
            );

            await db.SaveChangesAsync();
        }
    }
}
