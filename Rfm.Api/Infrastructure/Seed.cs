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
        var adminPass = "Admin123$";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var res = await userManager.CreateAsync(adminUser, adminPass);
            if (!res.Succeeded) throw new Exception("Failed to create default admin: " + string.Join(", ", res.Errors.Select(e => e.Description)));
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        
        var opEmail = "operator@gmail.com";
        var opPass = "Operator@123";
        var opUser = await userManager.FindByEmailAsync(opEmail);
        if (opUser == null)
        {
            opUser = new AppUser { UserName = opEmail, Email = opEmail, EmailConfirmed = true };
            var res = await userManager.CreateAsync(opUser, opPass);
            if (!res.Succeeded) throw new Exception("Failed to create default operator: " + string.Join(", ", res.Errors.Select(e => e.Description)));
            await userManager.AddToRoleAsync(opUser, "Operator");
        }

       
        var hasTO = await db.TourOperators.AnyAsync(to => to.IdentityUserId == opUser.Id);
        if (!hasTO)
        {
            var economy = await db.BookingClasses.FirstAsync(b => b.Name == "Economy");
            var anySeason = await db.Seasons.FirstOrDefaultAsync(); 

            var to = new TourOperator
            {
                Name = "Demo Operator",
                Email = opEmail,
                IdentityUserId = opUser.Id,
                BookingClasses = new List<BookingClass> { economy },
                TourOperatorSeasons = new List<TourOperatorSeason>()
            };

            if (anySeason != null)
            {
                to.TourOperatorSeasons.Add(new TourOperatorSeason { SeasonId = anySeason.Id });
            }

            db.TourOperators.Add(to);
            await db.SaveChangesAsync();
        }
    }
}
