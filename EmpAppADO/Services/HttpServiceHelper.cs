using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace EmpAppADO.Services
{
    public class HttpServiceHelper
    {
        private readonly HttpClient _http;
        private readonly ISessionCookieHelper _sessionHelper;
        private readonly ILogger<HttpServiceHelper> _logger;

        public HttpServiceHelper(HttpClient httpClient, ISessionCookieHelper sessionHelper, ILogger<HttpServiceHelper> logger)
        {
            _http = httpClient;
            _sessionHelper = sessionHelper;
            _logger = logger;
            _http.BaseAddress = new Uri("https://localhost:44397");
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await _sessionHelper.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _http.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<T> GetAsync<T>(string url)
        {
            await SetAuthorizationHeaderAsync();

            try
            {
                var response = await _http.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await _sessionHelper.ClearTokenAsync();
                    throw new UnauthorizedAccessException("Token has expired or is invalid");
                }

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }

                _logger.LogWarning("API call failed with status: {StatusCode}", response.StatusCode);
                return default;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP request failed: {Error}", ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T model)
        {
            await SetAuthorizationHeaderAsync();

            try
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _http.PostAsync(endpoint, content);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await _sessionHelper.ClearTokenAsync();
                    throw new UnauthorizedAccessException("Token has expired or is invalid");
                }

                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP POST failed: {Error}", ex.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            await SetAuthorizationHeaderAsync();

            try
            {
                var response = await _http.DeleteAsync(endpoint);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    await _sessionHelper.ClearTokenAsync();
                    throw new UnauthorizedAccessException("Token has expired or is invalid");
                }

                response.EnsureSuccessStatusCode();
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("HTTP DELETE failed: {Error}", ex.Message);
                throw;
            }
        }

        // for login 
        public async Task<HttpResponseMessage> PostWithoutAuth<T>(string endpoint, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }

}

