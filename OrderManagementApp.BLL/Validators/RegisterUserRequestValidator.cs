using FluentValidation;
using OrderManagementApp.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator() 
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be atleast 3 characters.")
                .MaximumLength(50).WithMessage("Username can not exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be atleast 6 characters.")
                .MaximumLength(50).WithMessage("Password can not exceed 50 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address")
                .MaximumLength(50).WithMessage("Email address can not exceed 50 characters.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .MaximumLength(50).WithMessage("Role can not exceed 50 characters.");
        }
    }
}
