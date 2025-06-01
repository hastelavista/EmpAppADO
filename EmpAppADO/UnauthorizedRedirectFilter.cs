using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmpAppADO
{
    public class UnauthorizedRedirectFilter : IExceptionFilter
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnauthorizedRedirectFilter(IHttpContextAccessor contextAccessor)
        {
            _httpContextAccessor = contextAccessor;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is UnauthorizedAccessException)
            {
                var httpContext = _httpContextAccessor.HttpContext;

                // Clear cookie
                httpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();

                // Redirect to login
                context.Result = new RedirectToActionResult("Login", "Account", null);
                context.ExceptionHandled = true;
            }
        }
    }
}
