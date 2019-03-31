using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CallsHistory.Services;
using CallsHistory.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CallsHistory.Controllers
{
    public class AccountController : Controller
    {
        private readonly LdapService ldapService;

        public AccountController(LdapService authService)
        {
            ldapService = authService;
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        public IActionResult AccessDeniedPath(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            TempData["warning"] = "У вас отсутствует доступ к данному разделу";
            return View("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ldapService.Login(model.Username, model.Password))
                    {
                        var userClaims = new List<Claim>
                        {
                            new Claim("username", model.Username),
                            new Claim(ClaimsIdentity.DefaultNameClaimType, model.Username)
                        };

                        var principal = new ClaimsPrincipal(new ClaimsIdentity(userClaims, ldapService.GetType().Name, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        return Redirect(returnUrl ?? Request.PathBase);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
