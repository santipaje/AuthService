using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs.Requests;
using FluentValidation;

namespace AuthService.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            // Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidatorMessages.EmailRequired)
                .EmailAddress().WithMessage(ValidatorMessages.InvalidEmailFormat);
            // Name
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(ValidatorMessages.NameRequired)
                .MinimumLength(2).WithMessage(ValidatorMessages.NameRequiresMinLenght);

            // Username
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(ValidatorMessages.UsernameRequired)
                .MinimumLength(2).WithMessage(ValidatorMessages.UsernameRequiredMinLenght);

            // Password
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(10)
                .Matches("[A-Z]").WithMessage(ValidatorMessages.PasswordRequiresUppercase)
                .Matches("[a-z]").WithMessage(ValidatorMessages.PasswordRequiresLowercase)
                .Matches("[0-9]").WithMessage(ValidatorMessages.PasswordRequiresDigit)
                .Matches(@"[^\da-zA-z]").WithMessage(ValidatorMessages.PasswordRequiresNonAlphanumeric);
        }
    }
}
