using ConstructApp.Models;
using Microsoft.AspNetCore.Identity;
using ConstructApp.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ConstructApp.Seeds
{
    [Authorize(Roles = "Admin")]
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Technician.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Director.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Cashier.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.FinanceManager.ToString()));
        }
    }
}
