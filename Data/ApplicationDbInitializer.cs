using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sparkur.Data;
using Sparkur.Models;

namespace Sparkur.Data
{
	public static class ApplicationDbInitializer
	{ 
		public static void SeedAdmin(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
		{
			context.Database.Migrate();

			string admin_email = config.GetValue<string>("Superadmin:Email");
			string admin_password = config.GetValue<string>("Superadmin:Password");

			if (userManager.FindByEmailAsync(admin_email).Result == null)
			{
				ApplicationUser user = new ApplicationUser
				{
					UserName = admin_email,
					Email = admin_email
				};

				IdentityResult result = userManager.CreateAsync(user, admin_password).Result;

				if (result.Succeeded)
				{
					userManager.AddToRoleAsync(user, "Admin").Wait();
				}
			}
		}
	}
}