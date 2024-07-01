using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Helpers;
using ConstructApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;


namespace ConstructApp.Seeds
{
    [Authorize(Roles = "Admin")]
    public static class DefaultUsers
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "aidanconstruction123@gmail.com",
                Email = "aidanconstruction123@gmail.com",
                FirstName = "Aidan",
                LastName = "Construction",
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                CanRequestForSomeone = true,
                CanApproved = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Admin@123");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Technician.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Director.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.Cashier.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Roles.FinanceManager.ToString());
                }
                await roleManager.SeedClaimsForAdmin();
            }
        }
        private async static Task SeedClaimsForAdmin(this RoleManager<IdentityRole> roleManager)
        {
            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new IdentityRole("Admin");
                await roleManager.CreateAsync(adminRole);
            }
            await roleManager.AddPermissionClaim(adminRole, "UserPermissions");
            await roleManager.AddPermissionClaim(adminRole, "ExpensePermissions");
            await roleManager.AddPermissionClaim(adminRole, "ApprovalPermissions");
            await roleManager.AddPermissionClaim(adminRole, "ProjectPermissions");
            await roleManager.AddPermissionClaim(adminRole, "ProjectMaterialPermissions");
            await roleManager.AddPermissionClaim(adminRole, "ProjectToolPermissions");


        }
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Constants.Permissions.GeneratePermissionsForModule(module);
            foreach (var permission in allPermissions)
            {
                if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                }
            }
        }
    }
}
