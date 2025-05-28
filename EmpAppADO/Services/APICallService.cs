using System.Text;
using EmpAppADO.UIModel;
using Newtonsoft.Json;

namespace EmpAppADO.Services
{
    public class APICallService
    {
        private readonly HttpServiceHelper _httpHelper;

        public APICallService(HttpServiceHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task<List<Dictionary<string, object>>> GetAllEmployeesAsync(string token)
        {
            return await _httpHelper.GetAsync<List<Dictionary<string, object>>>("/api/Emp/all", token);
        }

        public async Task<EmpExpForm> GetEmployeesAsyncByID(int id, string token)
        {
            return await _httpHelper.GetAsync<EmpExpForm>($"/api/Emp/{id}", token);
        }

        public async Task<HttpResponseMessage> AddNewEmpAsync(EmpExpForm model, string token)
        {
            return await _httpHelper.PostAsync("/api/Emp/add", model, token);
        }

        public async Task<HttpResponseMessage> UpdateEmpAsync(EmpExpForm model, string token)
        {
            return await _httpHelper.PostAsync("/api/Emp/update", model, token);
        }

        public async Task<HttpResponseMessage> DeleteEmpAsync(int id, string token)
        {
            return await _httpHelper.DeleteAsync($"/api/Emp/{id}", token);
        }

    }
}

