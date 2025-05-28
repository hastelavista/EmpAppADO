using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace EmpAppADO
{
    public class HttpServiceHelper
    {

        private readonly HttpClient _http;

        public HttpServiceHelper(HttpClient httpClient)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri("https://localhost:44397");
            //_http.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<T> GetAsync<T>(string url, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
            return default;
        }

        //default
        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T model, string? token = null)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _http.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return response;
        }


        public static class JwtHelper
        {
            private static JwtSecurityToken? ParseToken(string? token)
            {
                if (string.IsNullOrWhiteSpace(token)) return null;

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    return handler.ReadJwtToken(token);
                }
                catch
                {
                    return null;
                }
            }

            public static bool IsAdmin(string? token)
            {
                var jwt = ParseToken(token);
                return jwt?.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "true";
            }

            public static string? GetClaimValue(string? token, string claimType)
            {
                var jwt = ParseToken(token);
                return jwt?.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            }
        }



    }

}

