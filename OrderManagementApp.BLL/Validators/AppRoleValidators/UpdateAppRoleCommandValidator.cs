using FluentValidation;
using OrderManagementApp.BLL.Features.AppRoles.Commands;

namespace OrderManagementApp.BLL.Validators
{
    public class UpdateAppRoleCommandValidator : AbstractValidator<UpdateAppRoleCommand>
    {
        public UpdateAppRoleCommandValidator()
        {
            RuleFor(r => r.Id)
                .GreaterThan(0).WithMessage("Role Id is required.");

            RuleFor(r => r.RoleName)
                .NotEmpty().WithMessage("Role Name is required.")
                .MinimumLength(3).WithMessage("Role Name must be minimum 3 characters.")
                .MaximumLength(50).WithMessage("Role Name can not exceed 50 characters.");
        }
    }
}
