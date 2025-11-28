using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Application.Interfaces;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Mappers;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Serilog;

namespace AuthService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IValidator<RegisterRequestDto> _registerValidator;
        private readonly IValidator<LoginRequestDto> _loginValidator;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager, ITokenService tokenService, IValidator<RegisterRequestDto> registerValidator, IValidator<LoginRequestDto> loginValidator, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            Log.Information("Registration attempt for {Email}", registerDto.Email);

            var validation = await _registerValidator.ValidateAsync(registerDto);
            if (!validation.IsValid)
            {
                Log.Warning("Registration validations failed: {Errors}", validation.Errors.Select(error => error.ErrorMessage));
                throw new ValidationException(validation.Errors);
            }

            var existing = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existing != null)
            {
                Log.Warning("Registration failed: email {Email} already exists", registerDto.Email);
                return new RegisterResponseDto() { Succeeded = false, Errors = [ErrorMessages.EmailAlreadyRegistered] };
            }

            var user = new ApplicationUser()
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedTime = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errorDescriptionList = result.Errors.Select(error => error.Description).ToList();
                Log.Error("Identity creation errors for {Email}: {Errors}", registerDto.Email, string.Join(", ", errorDescriptionList));
                return new RegisterResponseDto() { Succeeded = false, Errors = errorDescriptionList };
            }

            await _userManager.AddToRoleAsync(user, RoleNames.User);

            Log.Information("Registration successful for {Email}", registerDto.Email);

            return new RegisterResponseDto() { Succeeded = true, Errors = [] };
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            Log.Information("Login attempt for {Email}", loginDto.Email);

            var validation = await _loginValidator.ValidateAsync(loginDto);
            if (!validation.IsValid)
            {
                Log.Warning("Login validations failed: {Errors}", validation.Errors.Select(error => error.ErrorMessage));
                throw new ValidationException(validation.Errors);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                Log.Warning("Login failed: user {Email} not found", loginDto.Email);
                return null;
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                Log.Warning("Login failed: invalid credentials for {Email}", loginDto.Email);
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = user.ToUserInfoDto(roles);
            var token = _tokenService.GenerateToken(userInfo);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes);

            Log.Information("Login successful for {Email}", loginDto.Email);

            return new LoginResponseDto() { AccessToken = token, ExpiresAt = expiresAt, RefreshToken = string.Empty };
        }
    }
}
