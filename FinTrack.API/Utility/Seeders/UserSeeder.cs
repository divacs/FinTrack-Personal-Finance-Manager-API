using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using FinTrack.Domain.Entities;

namespace FinTrack.API.Utility.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // --- ADMIN ---
            string adminEmail = "admin@fintrack.com";
            string adminPassword = "Admin123!";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // --- DEMO USER ---
            string demoEmail = "demo@fintrack.com";
            string demoPassword = "Demo123!";
            if (await userManager.FindByEmailAsync(demoEmail) == null)
            {
                var demoUser = new ApplicationUser
                {
                    UserName = "demo",
                    Email = demoEmail,
                    FirstName = "Demo",
                    LastName = "User",
                    PreferredCurrency = "USD",
                    MonthlyIncome = 3000,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(demoUser, demoPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(demoUser, "User");
                }
            }

            // --- OPTIONAL MANAGER ---
            string managerEmail = "manager@fintrack.com";
            string managerPassword = "Manager123!";
            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var managerUser = new ApplicationUser
                {
                    UserName = "manager",
                    Email = managerEmail,
                    FirstName = "Manager",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(managerUser, managerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }
        }
    }
}
