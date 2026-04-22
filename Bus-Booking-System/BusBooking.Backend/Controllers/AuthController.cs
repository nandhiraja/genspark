using BusBooking.Backend.DTOs;
using BusBooking.Backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BusBooking.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);
            if (!result) return BadRequest(new { message = "Registration failed or email already exists." });
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null) return Unauthorized(new { message = "Invalid email or password." });
            return Ok(response);
        }
    }
}
