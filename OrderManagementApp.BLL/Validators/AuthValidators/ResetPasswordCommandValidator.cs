using FluentValidation;
using OrderManagementApp.BLL.Features.Users.Commands;

namespace OrderManagementApp.BLL.Validators
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Reset token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be atleast 6 characters.")
                .MaximumLength(50).WithMessage("New password can not exceed 50 characters.");
        }
    }
}
