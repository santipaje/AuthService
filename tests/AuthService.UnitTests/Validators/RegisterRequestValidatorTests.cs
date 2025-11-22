
using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs.Requests;
using AuthService.Application.Validators;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService.UnitTests.Validators
{
    /// <summary>
    /// Testing class for the Register Request Validator
    /// </summary>
    public class RegisterRequestValidatorTests
    {
        private readonly RegisterRequestValidator _validator;

        public RegisterRequestValidatorTests()
        {
            _validator = new RegisterRequestValidator();
        }

        // Email

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new RegisterRequestDto { Email = string.Empty, FullName = "test-full-name", Password = "Password1123!", UserName = "test-user-name" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new RegisterRequestDto { Email = "not-an-email", FullName = "test-full-name", Password = "Password1123!", UserName = "test-user-name" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        // FullName

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            var model = new RegisterRequestDto { Email = "user@test.com", FullName = string.Empty, Password = "Password1123!", UserName = "test-user-name" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Too_Short()
        {
            var model = new RegisterRequestDto { Email = "user@test.com", FullName = "a", Password = "Password1123!", UserName = "test-user-name" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FullName);
        }

        // UserName

        [Fact]
        public void Should_Have_Error_When_UserName_Is_Empty()
        {
            var model = new RegisterRequestDto { Email = "user@test.com", FullName = "test-full-name", Password = "Password1123!", UserName = string.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserName);
        }

        [Fact]
        public void Should_Have_Error_When_UserName_Is_Too_Short()
        {
            var model = new RegisterRequestDto { Email = "user@test.com", FullName = "a", Password = "Password1123!", UserName = "a" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserName);
        }

        // Password

        private TestValidationResult<RegisterRequestDto> ValidatePassword(string password)
        {
            var model = new RegisterRequestDto { Email = "user@test.com", FullName = "test-full-name", Password = password, UserName = "test-user-name" };
            return _validator.TestValidate(model);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Pass1234!", "")]
        [InlineData("password1234!", ValidatorMessages.PasswordRequiresUppercase)]
        [InlineData("PASSWORD1234!", ValidatorMessages.PasswordRequiresLowercase)]
        [InlineData("Password!!!!", ValidatorMessages.PasswordRequiresDigit)]
        [InlineData("Password1234", ValidatorMessages.PasswordRequiresNonAlphanumeric)]
        public void Password_Should_Return_Specific_Error(string password, string expectedMessage)
        {
            var result = ValidatePassword(password);

            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage(expectedMessage);
        }


        // Correct model

        [Fact]
        public void Should_Not_Have_Errors_When_Request_Is_Valid()
        {
            var model = new RegisterRequestDto { Email = "user@test.com", FullName = "test-full-name", Password = "Password1123!", UserName = "test-user-name" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
