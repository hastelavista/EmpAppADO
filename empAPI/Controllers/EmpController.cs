using Microsoft.AspNetCore.Mvc;
using DATA.Repo;
using EmpAppADO.Models;

namespace EmpAppADO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpController : Controller
    {
        private readonly IEmpRepo _repo;
        public EmpController(IEmpRepo repo)
        {
            _repo = repo;
        }


        [HttpGet("all")]
        public async Task<IActionResult> ListEmployees()
        {
            var result = await _repo.GeEmployeeList();
            return Ok(result);

        }

        [HttpGet("{id?}")]
        public async Task<IActionResult> GetEmployee(int? id)
        {
            var model = new EmpExpFormView();

            if (id.HasValue)
            {
                var emp = await _repo.GetEmployeeByIdAsync(id.Value);

                if (emp != null)
                {
                    var exp = await _repo.GetExperiencesByEmployeeIdAsync(emp.EmployeeID);

                    model.Employee = emp;
                    model.Experiences = exp;

                }
                else
                {
                    return NotFound("Employee not found.");
                }
            }

            return Ok(model);
        }

        [HttpPost("add")]
        public async Task<IActionResult> InsertNewEmpExp([FromBody] EmpExpFormView model)
        {
            int employeeId = await _repo.InsertNewEmpExp(model.Employee, model.Experiences);
            return Ok(new { Message = "Employee inserted successfully.", EmployeeID = employeeId });
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateEmpExp([FromBody] EmpExpFormView model)
        {
            if (model == null) return BadRequest("Invalid data.");

            await _repo.UpdateEmployeeWithExperiences(model.Employee, model.Experiences);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            await _repo.DeleteEmpExpByID(id);
            return Ok();
        }
    }
}
