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

        public async Task<List<Dictionary<string, object>>> GetAllEmployeesAsync()
        {
            return await _httpHelper.GetAsync<List<Dictionary<string, object>>>("/api/Emp/all");
        }

        public async Task<EmpExpForm> GetEmployeesAsyncByID(int id)
        {
            return await _httpHelper.GetAsync<EmpExpForm>($"/api/Emp/{id}");
        }

        public async Task<HttpResponseMessage> AddNewEmpAsync(EmpExpForm model)
        {
            return await _httpHelper.PostAsync("/api/Emp/add", model);
        }

        public async Task<HttpResponseMessage> UpdateEmpAsync(EmpExpForm model)
        {
            return await _httpHelper.PostAsync("/api/Emp/update", model);
        }

        public async Task<HttpResponseMessage> DeleteEmpAsync(int id)
        {
            return await _httpHelper.DeleteAsync($"/api/Emp/{id}");
        }
    }
}