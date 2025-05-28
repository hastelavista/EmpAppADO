using EmpAppADO.Services;
using EmpAppADO.UIModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;


namespace EmpAppADO.UIController
{
    [Authorize]

    public class EmpViewController : Controller

    {
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
            string? token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Access denied. Please log in." });
            
            var employees = await _apiService.GetAllEmployeesAsync(token);
            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            string? token = HttpContext.Session.GetString("JWToken");

            var emp = await _apiService.GetEmployeesAsyncByID(id, token ?? "");

            if (!string.IsNullOrEmpty(emp.Employee.ImagePath))
            {
                emp.Employee.ImagePath = Url.Content($"~/uploads/{emp.Employee.ImagePath}");
            }
            return Ok(emp);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddNewEmp( EmpExpForm model)
        // {
        //    // Save image to server
        //    string? imagePath = null;
        //    if (model.ImageFile != null && model.ImageFile.Length > 0)
        //    {
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
        //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.ImageFile.CopyToAsync(stream);
        //        }

        //        imagePath = uniqueFileName;
        //    }

        //    model.Employee.ImagePath = imagePath;

        //    var response = await _apiService.AddNewEmpAsync(model);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<IActionResult> UpdateEmp(EmpExpForm model)
        //{
        //    if (model.ImageFile != null && model.ImageFile.Length > 0)
        //    {
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //        if (!Directory.Exists(uploadsFolder))
        //            Directory.CreateDirectory(uploadsFolder);

        //        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(model.ImageFile.FileName);
        //        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await model.ImageFile.CopyToAsync(stream);
        //        }

        //        model.Employee.ImagePath = uniqueFileName;
        //    }

        //    var response = await _apiService.UpdateEmpAsync(model);
        //    return Ok();
        //}

        [HttpPost]
        public async Task<IActionResult> AddNewEmp(EmpExpForm model)
        {
            string? token = HttpContext.Session.GetString("JWToken");

            model.Employee.ImagePath = await SaveImageAsync(model.ImageFile);

            var response = await _apiService.AddNewEmpAsync(model, token ?? "");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmp(EmpExpForm model)
        {
            string? token = HttpContext.Session.GetString("JWToken");

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                model.Employee.ImagePath = await SaveImageAsync(model.ImageFile);
            }
            var response = await _apiService.UpdateEmpAsync(model, token ?? "");
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteEmp(int id)
        {
            string? token = HttpContext.Session.GetString("JWToken");            
            var response = await _apiService.DeleteEmpAsync(id, token ?? "");

            return Ok();
        }
    }
}
