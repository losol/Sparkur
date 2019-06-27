using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sparkur.Data;
using Sparkur.Models;

namespace Sparkur.Data
{
    public static class ApplicationDbInitializer
    {
        public async static void SeedAdmin(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            // Add administrator role if it does not exist
			string[] roleNames = { "Admin", "SuperAdmin" };
			IdentityResult roleResult;
			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Add super-admin if none exists
			if (!userManager.GetUsersInRoleAsync("SuperAdmin").Result.Any())
			{
				_ = config.GetValue<string>("Superadmin:Email") ?? throw new ArgumentException("SuperAdmin email not set. Please check install documentation");
				_ = config.GetValue<string>("Superadmin:Password") ?? throw new ArgumentException("SuperAdmin password not set. Please check install documentation");

				var user = await userManager.FindByEmailAsync(config.GetValue<string>("Superadmin:Email"));

				if (user == null)
				{
					var superadmin = new ApplicationUser
                    {
						UserName = config.GetValue<string>("Superadmin:Email"),
						Email = config.GetValue<string>("Superadmin:Email"),
						EmailConfirmed = true
					};
					string UserPassword = config.GetValue<string>("Superadmin:Password");
					var createSuperAdmin = await userManager.CreateAsync(superadmin, UserPassword);
					if (createSuperAdmin.Succeeded)
					{
						await userManager.AddToRoleAsync(superadmin, "SuperAdmin");
					}
				}
            }
        }
    }
}