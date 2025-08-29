using MarketZone.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MarketZone.Infrastructure.Identity.Seeds
{
    public static class DefaultBasicUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "abd",
                Email = "Admin@Admin.com",
                Name = "abd",
                PhoneNumber = "099",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (!await userManager.Users.AnyAsync())
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "123123");
                }
            }
        }
    }
}
