using Azure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniqIo.Models;
using UniqIo.ViewModel.Auths;

namespace UniqIo.Controllers;

public class AccountController(UserManager<AppUser>_userManager,SignInManager<AppUser> _signInManager) : Controller
{
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        AppUser user = new AppUser()
        {
            Fullname = vm.Fullname,
            Email = vm.Email,
            UserName=vm.Username
        };
         var result = await _userManager.CreateAsync(user,vm.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        if (!ModelState.IsValid) 
        {
            return View();
        }
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm,string? returnUrl=null)
    {
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
        if(user is null)
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
        if(string.IsNullOrWhiteSpace(returnUrl))
        return RedirectToAction("Index", "Home");
        return LocalRedirect(returnUrl);
    }
     
}
