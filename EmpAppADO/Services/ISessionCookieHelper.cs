using System.Security.Claims;
using System.Threading.Tasks;

namespace EmpAppADO.Services
{
    public interface ISessionCookieHelper
    {
        Task<string?> GetTokenAsync();
        Task SetTokenAsync(string token);
        Task ClearTokenAsync();
        Task<bool> IsTokenValidAsync();
        ClaimsPrincipal BuildClaimsPrincipal(string username, string token);



    }
}
