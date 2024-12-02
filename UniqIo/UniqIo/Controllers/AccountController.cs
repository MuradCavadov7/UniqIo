using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniqIo.Models;
using UniqIo.ViewModel.Auths;

namespace UniqIo.Controllers;

public class AccountController(UserManager<AppUser>_userManager) : Controller
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
     
}
