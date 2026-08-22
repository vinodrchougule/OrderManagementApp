using FluentValidation;
using OrderManagementApp.BLL.Features.Users.Commands;

namespace OrderManagementApp.BLL.Validators
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(50).WithMessage("Username can not exceed 50 characters.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be atleast 6 characters.")
                .MaximumLength(50).WithMessage("New password can not exceed 50 characters.");

            RuleFor(x => x.NewPassword)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("New password must be different from the current password.")
                .When(x => !string.IsNullOrEmpty(x.NewPassword) && !string.IsNullOrEmpty(x.CurrentPassword));
        }
    }
}
