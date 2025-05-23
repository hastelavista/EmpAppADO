using Newtonsoft.Json;
using System.Text;

namespace EmpAppADO
{
    public class HttpServiceHelper
    {

        private readonly HttpClient _http;

        public HttpServiceHelper(HttpClient httpClient)
        {
            _http = httpClient;
            _http.BaseAddress = new Uri("https://localhost:44397");
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _http.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T model)
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            var response = await _http.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return response;
        }
    }
}
