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
    }
}
