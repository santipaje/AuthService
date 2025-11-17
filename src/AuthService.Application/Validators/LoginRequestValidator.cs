using AuthService.Application.Common.Constants;
using AuthService.Application.DTOs.Requests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
    {
        public LoginRequestValidator()
        {
            // Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidatorMessages.EmailRequired)
                .EmailAddress().WithMessage(ValidatorMessages.InvalidEmailFormat);

            // Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ValidatorMessages.PasswordRequired);
        }
    }
}
