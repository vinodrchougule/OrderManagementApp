using FluentValidation;
using OrderManagementApp.BLL.Features.Users.Commands;

namespace OrderManagementApp.BLL.Validators
{
    public class UpdateAppUserCommandValidator : AbstractValidator<UpdateAppUserCommand>
    {
        public UpdateAppUserCommandValidator()
        {
            RuleFor(r => r.Id)
                .GreaterThan(0).WithMessage("User Id is required.");

            RuleFor(r => r.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MinimumLength(3).WithMessage("Username must be atleast 3 characters.")
                .MaximumLength(50).WithMessage("Username can not exceed 50 characters.");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.")
                .MaximumLength(50).WithMessage("Email address can not exceed 50 characters.");

            RuleFor(r => r.Role)
                .NotEmpty().WithMessage("Role is required.")
                .MaximumLength(50).WithMessage("Role can not exceed 50 characters.");
        }
    }
}
