using FluentValidation;
using OrderManagementApp.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
