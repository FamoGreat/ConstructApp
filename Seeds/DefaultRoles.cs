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

        private static async Task SeedClaimsForAdmin(RoleManager<IdentityRole> roleManager, IdentityRole adminRole)
        {
            var permissions = new List<string>
            {
                nameof(Constants.Permissions.UserPermissions),
                nameof(Constants.Permissions.ExpensePermissions),
                nameof(Constants.Permissions.ApprovalPermissions),
                nameof(Constants.Permissions.ProjectPermissions),
                nameof(Constants.Permissions.ProjectMaterialPermissions),
                nameof(Constants.Permissions.ProjectToolPermissions)
            };

            foreach (var module in permissions)
            {
                var modulePermissions = Constants.Permissions.GeneratePermissionsForModule(module);
                foreach (var permission in modulePermissions)
                {
                    await roleManager.AddPermissionClaim(adminRole, permission);
                }
            }
        }
    }
}
