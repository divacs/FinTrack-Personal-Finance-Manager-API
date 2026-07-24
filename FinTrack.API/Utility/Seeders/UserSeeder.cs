using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using FinTrack.Domain.Entities;

namespace FinTrack.API.Utility.Seeders
{
    public static class UserSeeder
    {
        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            await EnsureUserWithRoleAsync(
                userManager,
                email: "admin@fintrack.com",
                password: "Admin123!",
                roleName: "Admin",
                userFactory: () => new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@fintrack.com",
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true
                });

            await EnsureUserWithRoleAsync(
                userManager,
                email: "demo@fintrack.com",
                password: "Demo123!",
                roleName: "User",
                userFactory: () => new ApplicationUser
                {
                    UserName = "demo",
                    Email = "demo@fintrack.com",
                    FirstName = "Demo",
                    LastName = "User",
                    PreferredCurrency = "USD",
                    MonthlyIncome = 3000,
                    EmailConfirmed = true
                });

            await EnsureUserWithRoleAsync(
                userManager,
                email: "manager@fintrack.com",
                password: "Manager123!",
                roleName: "Manager",
                userFactory: () => new ApplicationUser
                {
                    UserName = "manager",
                    Email = "manager@fintrack.com",
                    FirstName = "Manager",
                    LastName = "User",
                    EmailConfirmed = true
                });
        }

        private static async Task EnsureUserWithRoleAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string roleName,
            Func<ApplicationUser> userFactory)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = userFactory();
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
