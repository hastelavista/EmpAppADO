using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DATA.Models;
using DATA.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace empAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : Controller
    {
        private readonly string _jwtKey;
        private readonly IUserRepo _repo;

        public LoginController(IConfiguration config, IUserRepo repo)
        {
            _jwtKey = config["JwtSettings:Key"];
            _repo = repo;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User model)
        {
          var user = await _repo.GetUserByUsernameAndPassword(model.Username, model.Password);
          if (user == null)
                return Unauthorized("Invalid username or password");
            
          var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
          var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("IsAdmin", user.IsAdmin.ToString().ToLower())  
            };

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                isAdmin = user.IsAdmin
            });
        }


    }
}
