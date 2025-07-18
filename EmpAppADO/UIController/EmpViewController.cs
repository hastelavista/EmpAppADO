﻿using EmpAppADO.Services;
using EmpAppADO.UIModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;


namespace EmpAppADO.UIController
{
    [Authorize]

    public class EmpViewController : Controller
    {


        private readonly APICallService _apiService;

        public EmpViewController(APICallService apiService)
        {
            _apiService = apiService;
        }


        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return uniqueFileName;
            }
            return null;
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
            var emp = await _apiService.GetEmployeesAsyncByID(id);

            if (!string.IsNullOrEmpty(emp.Employee.ImagePath))
            {
                emp.Employee.ImagePath = Url.Content($"~/uploads/{emp.Employee.ImagePath}");
            }
            return Ok(emp);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewEmp(EmpExpForm model)
        {
            model.Employee.ImagePath = await SaveImageAsync(model.ImageFile);
            var response = await _apiService.AddNewEmpAsync(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmp(EmpExpForm model)
        {
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                model.Employee.ImagePath = await SaveImageAsync(model.ImageFile);
            }
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
