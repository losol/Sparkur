using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sparkur.Data;
using Sparkur.Models;

namespace Sparkur.Services
{
    public abstract class SeedDatabase
    {
        protected readonly ApplicationDbContext _db;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _config;

		public SeedDatabase(ApplicationDbContext db,
			RoleManager<IdentityRole> roleManager,
			UserManager<ApplicationUser> userManager,
			IConfiguration config)
		{
			_db = db;
			_config = config;
			_roleManager = roleManager;
			_userManager = userManager;
		}

        public virtual async Task SeedAsync()
        {
            // Add administrator role if it does not exist
			string[] roleNames = { "Admin", "SuperAdmin" };
			IdentityResult roleResult;
			foreach (var roleName in roleNames)
			{
				var roleExist = await _roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// Add super-admin if none exists
			if (!_userManager.GetUsersInRoleAsync("SuperAdmin").Result.Any())
			{
				_ = _config.GetValue<string>("Superadmin:Email") ?? throw new ArgumentException("SuperAdmin email not set. Please check install documentation");
				_ = _config.GetValue<string>("Superadmin:Password") ?? throw new ArgumentException("SuperAdmin password not set. Please check install documentation");

				var user = await _userManager.FindByEmailAsync(_config.GetValue<string>("Superadmin:Email"));

				if (user == null)
				{
					var superadmin = new ApplicationUser
                    {
						UserName = _config.GetValue<string>("Superadmin:Email"),
						Email = _config.GetValue<string>("Superadmin:Email"),
						EmailConfirmed = true
					};
					string UserPassword = _config.GetValue<string>("Superadmin:Password");
					var createSuperAdmin = await _userManager.CreateAsync(superadmin, UserPassword);
					if (createSuperAdmin.Succeeded)
					{
						await _userManager.AddToRoleAsync(superadmin, "SuperAdmin");
					}
				}
            }
        }
    }
}