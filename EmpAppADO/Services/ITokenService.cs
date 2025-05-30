using System.Threading.Tasks;

namespace EmpAppADO.Services
{
    public interface ITokenService
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task ClearTokenAsync();
        Task<bool> IsTokenValidAsync();
        Task<string?> GetClaimValueAsync(string claimType);
        Task<bool> IsUserAdminAsync();
        Task<bool> RefreshTokenIfNeededAsync();
    }
}
