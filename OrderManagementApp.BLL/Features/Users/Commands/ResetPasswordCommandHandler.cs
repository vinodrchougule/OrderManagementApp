using FluentValidation;
using MediatR;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IAppUserRepository _appUserRepository;

        public ResetPasswordCommandHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _appUserRepository.GetByValidPasswordResetTokenAsync(request.Token, cancellationToken);

            if (user is null)
                throw new ValidationException("Password reset link is invalid or has expired.");

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var updated = await _appUserRepository.ResetPasswordAsync(user.Id, newPasswordHash, cancellationToken);

            return updated;
        }
    }
}
