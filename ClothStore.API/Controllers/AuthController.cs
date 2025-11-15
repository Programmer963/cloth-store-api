using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ClothStore.API.Controllers.Dtos;
using ClothStore.API.Services;
using ClothStore.Core.Entities;

namespace ClothStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AspNetUser> _signInManager;
        private readonly UserManager<AspNetUser> _userManager;
        private readonly JwtHelper _jwtHelper;

        public AuthController(
            SignInManager<AspNetUser> signInManager,
            UserManager<AspNetUser> userManager,
            JwtHelper jwtHelper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "User not found." });
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return Unauthorized(new { message = "Incorrect password." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHelper.GenerateJwtToken(user, roles);

            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new AspNetUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "CUSTOMER");
                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtHelper.GenerateJwtToken(user, roles);

                return Ok(new { Token = token });
            }

            return BadRequest(result.Errors);
        }
    }
}

