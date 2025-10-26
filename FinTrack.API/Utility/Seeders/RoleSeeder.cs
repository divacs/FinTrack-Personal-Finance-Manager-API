using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace FinTrack.API.Utility.Seeders
{
    public static class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Definisane role za FinTrack
            string[] roles = { "Admin", "User", "Manager" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
