using FluentValidation;
using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Validators
{
    public class CreateAppRoleRequestValidator : AbstractValidator<CreateAppRoleRequest>
    {
        public CreateAppRoleRequestValidator()
        {
            RuleFor(r => r.RoleName)
                .NotEmpty().WithMessage("Role Name is required.")
                .MinimumLength(3).WithMessage("Role Name must be minimum 3 characters.")
                .MaximumLength(50).WithMessage("Role Name can not exceed 50 characters.");
        }
    }
}
