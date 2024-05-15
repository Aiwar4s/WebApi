using Microsoft.AspNetCore.Identity;
using WebApi.Auth.Entity;

namespace WebApi.Auth;

public class AuthDbSeeder
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthDbSeeder(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await AddDefaultRoles();
        await AddAdminUser();
    }

    private async Task AddDefaultRoles()
    {
        foreach (string role in UserRoles.All)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private async Task AddAdminUser()
    {
        User newAdminUser = new User
        {
            UserName = "admin",
            Email = "admin@admin.com"
        };
        User? existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
        if (existingAdminUser == null)
        {
            IdentityResult createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "Admin.123");
            if (createAdminUserResult.Succeeded)
            {
                await _userManager.AddToRolesAsync(newAdminUser, UserRoles.All);
            }
        }
    }
}
