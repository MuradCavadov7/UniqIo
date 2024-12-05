using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniqIo.Extension;
using UniqIo.Models;
using UniqIo.ViewModel.Auths;
using UniqIo.Views.Account.Enums;

namespace UniqIo.Controllers;

public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager) : Controller
{
    private bool isAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;
    public IActionResult Register()
	{
		if(isAuthenticated) return RedirectToAction("Index","Home");
		return View();
	}
	[HttpPost]
	public async Task<IActionResult> Register(RegisterVM vm)
	{
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid)
		{
			return View();
		}
		AppUser user = new AppUser()
		{
			Fullname = vm.Fullname,
			Email = vm.Email,
			UserName = vm.Username
		};
		var result = await _userManager.CreateAsync(user, vm.Password);
		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
		}
		var roleResult = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
		if (!roleResult.Succeeded)
		{
			foreach (var err in roleResult.Errors)
			{
				ModelState.AddModelError("", err.Description);
			}
			return View();
		}
		return View();
	}
	//public async Task<IActionResult> Role()
	//{
	//	foreach (Roles item in Enum.GetValues(typeof(Roles)))
	//	{
	//		await _roleManager.CreateAsync(new IdentityRole(item.GetRole()));
	//	}
	//	return Ok();
	//}
	public IActionResult Login()
	{
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        return View();
	}
	[HttpPost]
	public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
	{
        if (isAuthenticated) return RedirectToAction("Index", "Home");
        if (!ModelState.IsValid) return View();
		AppUser? user = null;
		if (vm.UsernameOrEmail.Contains("@"))
		{
			user = await _userManager.FindByEmailAsync(vm.UsernameOrEmail);
		}
		else
		{
			user = await _userManager.FindByNameAsync(vm.UsernameOrEmail);
		}
		if (user is null)
		{
			ModelState.AddModelError("", "Username or Password is Wrong");
			return View();
		}
		var result = await _signInManager.PasswordSignInAsync(user, vm.Password, vm.RememberMe, true);
		if (!result.Succeeded)
		{
			ModelState.AddModelError("", "Wait until" + user.LockoutEnd!.Value.ToString("yyyy-MM-dd HH:mm:ss"));
			return View();
		}
		if (result.IsNotAllowed)
		{
			ModelState.AddModelError("", "Username or Password is Wrong");
			return View();
		}
		if (string.IsNullOrWhiteSpace(returnUrl))
		{
			if(await _userManager.IsInRoleAsync(user, "Admin"))
			{
				return RedirectToAction("Index", new { Controller = "Dashboard", Area = "Admin" });
			}
			return RedirectToAction("Index", "Home");
		}
		return LocalRedirect(returnUrl);
	}

	[Authorize]
	public async Task<IActionResult> LogOut()
	{
		await _signInManager.SignOutAsync();
		return RedirectToAction(nameof(Login));
	}

}
