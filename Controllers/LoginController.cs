﻿using MagazinAE.Models.VMs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace MagazinAE.Controllers
{
    public class LoginController : Controller
    {
        private readonly MagazinAEContext context;
        public LoginController(MagazinAEContext context) 
        {
            this.context = context;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "There were some errors in your form");
                return View(loginVM);
            }

            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == loginVM.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View(loginVM);
            }

            if (user.Password != loginVM.Password)
            {
                ModelState.AddModelError(string.Empty, "Invalid Credentials");
                return View(loginVM);
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, loginVM.Username),
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = false,
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

    }
}
