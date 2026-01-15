using Microsoft.AspNetCore.Identity;

namespace CustomerCampaignService.Seeds;

public static class IdentitySeed
{
    public static async Task IdentitySeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager)
    {
        if (!roleManager.Roles.Any())
        {
            var roles = new[] { "Admin", "Agent" };
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        if (!userManager.Users.Any())
        {
            var admin = new IdentityUser { UserName = "admin" };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");

            var agent = new IdentityUser { UserName = "agent1" };
            await userManager.CreateAsync(agent, "Agent123!");
            await userManager.AddToRoleAsync(agent, "Agent");
        }
    }
}
