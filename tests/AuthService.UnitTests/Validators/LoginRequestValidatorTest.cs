using AuthService.Application.DTOs.Requests;
using AuthService.Application.Validators;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService.UnitTests.Validators
{
    /// <summary>
    /// Testing class for the Login Request Validator
    /// </summary>
    public class LoginRequestValidatorTest
    {
        private readonly LoginRequestValidator _validator;

        public LoginRequestValidatorTest()
        {
            _validator = new LoginRequestValidator();
        }

        // Email

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var model = new LoginRequestDto { Email = string.Empty, Password = "Password1!"};
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new LoginRequestDto { Email = "not-an-email", Password = "Password1!" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        // Password

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var model = new LoginRequestDto { Email = "user@test.com", Password = string.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        // Correct model

        [Fact]
        public void Should_Not_Have_Errors_When_Request_Is_Valid()
        {
            var model = new LoginRequestDto { Email = "user@test.com", Password = "Password1!" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
