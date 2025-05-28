using DATA.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace empAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        private readonly IUserRepo _repo;
        public UserController(IUserRepo repo)
        {
            _repo = repo;
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetUsers()
        {

            var isAdmin = User.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value;
            if (isAdmin != "true")
                return StatusCode(StatusCodes.Status403Forbidden, "Not Authorized");

            var result = await _repo.GetAllUsers();
            return Ok(result);
        }
    }
}
