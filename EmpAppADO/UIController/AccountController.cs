﻿using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using EmpAppADO.UIModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using EmpAppADO.Services;

namespace EmpAppADO.UIController
{
    public class AccountController : Controller
    {
        private readonly HttpServiceHelper _httpService;
        private readonly ITokenService _tokenService;

        public AccountController(HttpServiceHelper httpService, ITokenService tokenService)
        {
            _httpService = httpService;
            _tokenService = tokenService;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var response = await _httpService.PostWithoutAuth("api/Login/login", model);
                var json = await response.Content.ReadAsStringAsync();
                var tokenObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                if (tokenObject != null && tokenObject.TryGetValue("token", out var token))
                {
                    // Use centralized token service
                    await _tokenService.SetTokenAsync(token);

                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, model.username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("List", "EmpView");
                }

                ViewBag.Message = "Token was not returned from API.";
                return View("login");
            }
            catch (HttpRequestException ex)
            {
                ViewBag.Message = $"Login failed: {ex.Message}";
                return View("login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _tokenService.ClearTokenAsync();
            return RedirectToAction("login", "Account");
        }
    }
}