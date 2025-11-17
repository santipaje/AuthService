using AuthService.Application.DTOs.Requests;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerDTO)
        {
            var registerResponse = await _authService.RegisterAsync(registerDTO);
            if (!registerResponse.Succeeded) { return BadRequest(new { registerResponse.Errors }); }
            return Created(string.Empty, null);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginDTO)
        {
            var authResponse = await _authService.LoginAsync(loginDTO);
            if (authResponse?.AccessToken == null) { return Unauthorized(); }
            return Ok(authResponse.AccessToken);
        }
    }
}
