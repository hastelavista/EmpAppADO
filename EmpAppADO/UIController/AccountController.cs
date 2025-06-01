using EmpAppADO.Services;
using EmpAppADO.UIModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmpAppADO.UIController
{
    public class AccountController : Controller
    {
        private readonly APICallService _apiService;
        private readonly ISessionCookieHelper _sessionHelper;

        public AccountController(APICallService apiService, ISessionCookieHelper sessionHelper)
        {
            _apiService = apiService;
            _sessionHelper = sessionHelper;
        }


        public IActionResult Login() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var token = await _apiService.Login(model);
                if (string.IsNullOrEmpty(token))
                {
                    ViewBag.Message = "Login failed. Token not returned.";
                    return View("Login");
                }

                await _sessionHelper.SetTokenAsync(token);

                var principal = _sessionHelper.BuildClaimsPrincipal(model.username, token);
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

                return RedirectToAction("Dashboard", "Account");
            }

            catch (HttpRequestException ex)
            {
                ViewBag.Message = $"Login failed: {ex.Message}";
                return View("Login");
            }

        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await _sessionHelper.ClearTokenAsync();
            return RedirectToAction("login", "Account");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Dashboard()
        {
            var username = User.Identity?.Name ?? "Guest";
            var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value ?? "false";

            ViewBag.Username = username;
            ViewBag.IsAdmin = isAdmin;

            return View();
        }


    }
}