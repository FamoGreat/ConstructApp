using ConstructApp.Constants;
using ConstructApp.Data;
using ConstructApp.Models;
using Microsoft.AspNetCore.Identity;
using System;

namespace ConstructApp.Seeds
{
    public class DefaultUsers
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "ismailu@gmail.com",
                Email = "ismail@gmail.com",
                FirstName = "Ismail",
                LastName = "Batil",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false
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
            }
        }

 
    }
}
