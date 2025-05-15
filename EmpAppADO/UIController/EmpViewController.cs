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
        public async Task<IActionResult> AddNewEmp( EmpExpForm model)
         {
            // Save image to server
            string? imagePath = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = uniqueFileName;
            }

            model.Employee.ImagePath = imagePath;

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
