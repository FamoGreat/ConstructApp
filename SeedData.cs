using ConstructApp.Data;
using ConstructApp.Models;
using ConstructApp.Seeds;
using Microsoft.AspNetCore.Identity;

namespace ConstructApp
{
    public static class SeedData
    {
        public async static Task<WebApplication> Seed(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Seed Roles 
                    await DefaultRoles.SeedRolesAsync(userManager, roleManager);
                    // Seed Default User
                    await DefaultUsers.SeedAdminAsync(userManager, roleManager);
                    DefaultExpenseType.SeedExpenseType(dbContext);
                    DefaultMaterial.SeedMaterials(dbContext);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            return app;
        }
    }
}
