using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace EmpAppADO.Services
{
    public class TokenService: ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;
        private const string TokenSessionKey = "JWToken";

        public TokenService(IHttpContextAccessor httpContextAccessor, ILogger<TokenService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string?> GetTokenAsync()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(TokenSessionKey);
        }

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

        public async Task<bool> IsTokenValidAsync()
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token validation failed: {Error}", ex.Message);
                return false;
            }
        }

        public async Task<string?> GetClaimValueAsync(string claimType)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to extract claim {ClaimType}: {Error}", claimType, ex.Message);
                return null;
            }
        }

        public async Task<bool> IsUserAdminAsync()
        {
            var isAdminValue = await GetClaimValueAsync("IsAdmin");
            return isAdminValue == "true";
        }

        public async Task<bool> RefreshTokenIfNeededAsync()
        {
         
            var isValid = await IsTokenValidAsync();

            if (!isValid)
            {
                await ClearTokenAsync();
                return false;
            }

            return true;
        }
    }
}
