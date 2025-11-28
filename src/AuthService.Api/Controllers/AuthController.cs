using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequestDto registerDTO)
        {
            var registerResponse = await _authService.RegisterAsync(registerDTO);
            if (!registerResponse.Succeeded) { return BadRequest(new { registerResponse.Errors }); }
            return CreatedAtAction(string.Empty, null);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginDTO)
        {
            var authResponse = await _authService.LoginAsync(loginDTO);
            if (authResponse?.AccessToken == null) { return Unauthorized(); }
            return Ok(authResponse);
        }
    }
}
