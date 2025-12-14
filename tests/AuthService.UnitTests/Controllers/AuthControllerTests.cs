using AuthService.Api.Controllers;
using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthService.UnitTests.Controllers
{
    /// <summary>
    /// Testing class for the Auth Controller
    /// </summary>
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _sut;

        public AuthControllerTests()
        {
            // Global Arrange
            _authServiceMock = new Mock<IAuthService>();
            _sut = new AuthController( _authServiceMock.Object );
        }

        [Fact]
        [Trait("Category", "Register")]
        public async Task RegisterAsync_ShouldReturnBadRequest_WhenEmailExists()
        {
            // Arrange
            var request = new RegisterRequestDto { Email = "user@test.com", Password = "Password1234!", FullName = "test-full-name", UserName = "test-user-name" };
            var failedResponse = new RegisterResponseDto() { Succeeded = false, Errors = [ErrorMessages.EmailAlreadyRegistered] };

            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync(failedResponse);

            // Act
            var result = await _sut.RegisterAsync(request);

            // Assert
            // Verify HTTP response type (201 Created)
            result.Should().BeOfType<BadRequestObjectResult>();

            // Verify body type
            var badRequestResult = result.As<BadRequestObjectResult>();
            badRequestResult.Value.Should().BeOfType<ApiValidationErrorResponse>();

            // Verify DTO content
            var actualResponseDto = badRequestResult.Value.As<ApiValidationErrorResponse>();
            actualResponseDto.Status.Should().Be(400);
            actualResponseDto.Errors.Should().Contain(ErrorMessages.EmailAlreadyRegistered);

            // Verify register method have been called
            _authServiceMock.Verify(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Register")]
        public async Task RegisterAsync_ShouldReturnCreated_WhenSucceeded()
        {
            // Arrange
            var request = new RegisterRequestDto { Email = "user@test.com", Password = "Password1234!", FullName = "test-full-name", UserName = "test-user-name" };

            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>()))
                .ReturnsAsync(new RegisterResponseDto { Succeeded = true, Errors = [] });

            // Act
            var result = await _sut.RegisterAsync(request);

            // Assert
            // Verify HTTP response type (201 Created)
            result.Should().BeOfType<CreatedAtActionResult>();

            // Verify register method have been called
            _authServiceMock.Verify(s => s.RegisterAsync(It.IsAny<RegisterRequestDto>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Login")]
        public async Task LoginAsync_ShouldReturnOk_WithLoginResponseDto()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "user@test.com", Password = "Password1234!" };
            var expectedResponse = new LoginResponseDto
            {
                AccessToken = "test-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                RefreshToken = string.Empty
            };

            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            // Verify HTTP response type (200 OK)
            result.Should().BeOfType<OkObjectResult>();

            // Verify body type
            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().BeOfType<LoginResponseDto>();

            // Verify DTO content
            var responseDto = okResult.Value.As<LoginResponseDto>();
            responseDto.AccessToken.Should().Be(expectedResponse.AccessToken);

            // Verify login method have been called
            _authServiceMock.Verify(x => x.LoginAsync(It.IsAny<LoginRequestDto>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Login")]
        public async Task LoginAsync_ShouldReturnUnauthorized_WhenServiceReturnsNull()
        {
            // Arrange
            var request = new LoginRequestDto { Email = "wrong@test.com", Password = "wrong" };

            _authServiceMock.Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync((LoginResponseDto?)null); // Retornar null

            // Act
            var result = await _sut.LoginAsync(request);

            // Assert
            // Verify HTTP response type (401 Unauthorized)
            result.Should().BeOfType<UnauthorizedResult>();

            // Verify login method have been called
            _authServiceMock.Verify(s => s.LoginAsync(It.IsAny<LoginRequestDto>()), Times.Once);
        }
    }
}