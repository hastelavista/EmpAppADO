using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace EmpAppADO.Services
{
    public class SessionCookieHelper: ISessionCookieHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SessionCookieHelper> _logger;
        private const string TokenSessionKey = "JWToken";


        public SessionCookieHelper(IHttpContextAccessor httpContextAccessor, ILogger<SessionCookieHelper> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task<string?> GetTokenAsync() =>
        Task.FromResult(_httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey));

        public async Task SetTokenAsync(string token)
        {
            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString(TokenSessionKey, token);
            }
        }

        public async Task ClearTokenAsync()
        {
            if (_httpContextAccessor.HttpContext?.Session != null)
            {
                _httpContextAccessor.HttpContext.Session.Remove(TokenSessionKey);
            }
        }

        public Task<bool> IsTokenValidAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);
            if (string.IsNullOrEmpty(token)) return Task.FromResult(false);

            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                return Task.FromResult(jwt.ValidTo > DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Invalid JWT token: {Error}", ex.Message);
                return Task.FromResult(false);
            }
        }

        public ClaimsPrincipal BuildClaimsPrincipal(string username, string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var isAdmin = jwt.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value ?? "false";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("IsAdmin", isAdmin)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }


    }
}
