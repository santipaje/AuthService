using AuthService.Application.DTOs.Requests;
using AuthService.Application.DTOs.Responses;
using AuthService.IntegrationTests.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;

namespace AuthService.IntegrationTests.AuthTests
{
    /// <summary>
    /// Contains a set of integration tests for the Authentication API endpoints
    /// </summary>
    /// <remarks>
    /// This class belongs to the <c>IntegrationTestsCollection</c>, ensuring it shares 
    /// a single, persistent instance of the <see cref="CustomWebApplicationFactory{TProgram}"/>.
    /// <br/>
    /// This allows tests to share the same in-memory database instance, enabling sequential 
    /// testing of user flows (e.g., Register then Login).
    /// </remarks>
    [Collection("IntegrationTestsCollection")]
    public class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        public AuthEndpointsTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        }

        [Fact]
        public async Task Register_ShouldReturn201_WhenRequestIsValid()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "registeruser@test.com",
                FullName = "Register test user",
                UserName = "registeruser",
                Password = "Password1234!"
            };

            //Act
            var response = await _client.PostAsJsonAsync(ApiRoutes.Auth.RegisterUri, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Register_ShouldReturn400_WhenPasswordIsInValid()
        {
            // Arrange
            var request = new RegisterRequestDto
            {
                Email = "user@test.com",
                FullName = "Integration user",
                UserName = "testuser",
                Password = "weak"
            };

            // Act
            var response = await _client.PostAsJsonAsync(ApiRoutes.Auth.RegisterUri, request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ReturnsToken_whenCredentialsAreValid()
        {
            // Arrange
            var registerRequest = new RegisterRequestDto
            {
                Email = "loginuser@test.com",
                FullName = "Login test user",
                UserName = "loginuser",
                Password = "Password1234!"
            };

            var registerResponse = await _client.PostAsJsonAsync(ApiRoutes.Auth.RegisterUri, registerRequest);

            var loginRequest = new LoginRequestDto 
            { 
                Email = "loginuser@test.com", 
                Password = "Password1234!" 
            };

            // Act
            var loginResponse = await _client.PostAsJsonAsync(ApiRoutes.Auth.LoginUri, loginRequest);
            var authContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();

            // Assert
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            authContent.Should().NotBeNull();
            authContent!.AccessToken.Should().NotBeNullOrEmpty();

            // JWT validation
            var jwt = _tokenHandler.ReadJwtToken(authContent.AccessToken);
            jwt.Issuer.Should().NotBeNullOrEmpty();
            jwt.Audiences.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturn401_WhenUserDoesNotExist()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "not@exists.com",
                Password = "WrongPass1234!"
            };

            // Act
            var response = await _client.PostAsJsonAsync(ApiRoutes.Auth.LoginUri, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_ShouldReturn401_WhenPasswordIsWrong()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "loginuser@test.com",
                Password = "WrongPass1234!"
            };

            // Act
            var response = await _client.PostAsJsonAsync(ApiRoutes.Auth.LoginUri, loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

    }
}
