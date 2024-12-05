using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniqIo.Models;
using UniqIo.Views.Account.Enums;

namespace UniqIo.Extension;

public static class SeedExtension
{
	public static void UseUserSeed(this IApplicationBuilder app)
	{
		using (var scope = app.ApplicationServices.CreateScope())
		{
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
			CreateRoles(roleManager).Wait();
			CreateUsers(userManager).Wait();
		}
	}
	private static async Task CreateRoles(RoleManager<IdentityRole> _roleManager)
	{
		if (!await _roleManager.Roles.AnyAsync())
		{
			foreach (Roles item in Enum.GetValues(typeof(Roles)))
			{
				await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));
			}
		}
	}
	private static async Task CreateUsers(UserManager<AppUser> _userManager)
	{
		if (!await _userManager.Users.AnyAsync(u => u.NormalizedUserName == "ADMIN"))
		{
			AppUser user = new AppUser();
			user.UserName = "admin";
			user.Email = "admin@gmail.com";
			user.Fullname = "admin";
			string role = nameof(Roles.Admin);
			await _userManager.CreateAsync(user, "admin123");
			await _userManager.AddToRoleAsync(user, role);
		}
	}
}