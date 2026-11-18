using KN.Core.Constants;
using KN.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KN.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp, IConfiguration config)
    {
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<AppUser>>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");
        
        // 1. Roles
        foreach (var role in new[] { Roles.Admin, Roles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
        
        // 2. Admin
        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            logger.LogWarning("SeedAdmin:Email / SeedAdmin:Password не заданы, админ не создан.");
        }
        else
        {
            var admin = await userManager.FindByEmailAsync(email);
            if (admin is null)
            {
                admin = new AppUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "Администратор",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, password);
                if (!result.Succeeded)
                {
                    logger.LogError("Не удалось создать админа: {Errors}", string.Join(",", result.Errors.Select(e => e.Description)));
                    admin = null;
                }
            }
            
            if (admin is not null && !await userManager.IsInRoleAsync(admin, Roles.Admin))
                await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
        
        // Всем по умолчанию роль user
        foreach (var user in userManager.Users.ToList())
        {
            if ((await userManager.GetRolesAsync(user)).Count == 0)
            {
                await userManager.AddToRoleAsync(user, Roles.User);
            }
        }
    }
}