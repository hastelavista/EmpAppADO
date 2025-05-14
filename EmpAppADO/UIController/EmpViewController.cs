using EmpAppADO.Services;
using EmpAppADO.UIModel;
using Microsoft.AspNetCore.Mvc;

namespace EmpAppADO.UIController
{
    public class EmpViewController : Controller

    {
        private readonly APICallService _apiService;
        public EmpViewController(APICallService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _apiService.GetAllEmployeesAsync();
            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _apiService.GetEmployeesAsyncByID(id);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmp([FromBody] EmpExpForm model)
        {
            var response = await _apiService.AddNewEmpAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmp([FromBody] EmpExpForm model)
        {
            var response = await _apiService.UpdateEmpAsync(model);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmp(int id)
        {
            var response = await _apiService.DeleteEmpAsync(id);
            return Ok();
        }
    }
}
