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
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        await db.Database.MigrateAsync();

       
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

        
        if (!await db.BookingClasses.AnyAsync())
        {
            db.BookingClasses.AddRange(
                new BookingClass { Name = "Economy" },
                new BookingClass { Name = "Business" }
            );
            await db.SaveChangesAsync();
        }

        
        var adminEmail = "admin@rfm.com";
        var adminPassword = "Admin123$";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
            else
            {
                throw new Exception("Failed to create default admin: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

       
        var operatorEmail = "operator@gmail.com";
        var operatorPassword = "Operator@123";

        var operatorUser = await userManager.FindByEmailAsync(operatorEmail);
        if (operatorUser == null)
        {
            operatorUser = new AppUser
            {
                UserName = operatorEmail,
                Email = operatorEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(operatorUser, operatorPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(operatorUser, "Operator");
            }
            else
            {
                throw new Exception("Failed to create default operator: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
