using FluentValidation;
using MediatR;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IAppUserRepository _appUserRepository;

        public ChangePasswordCommandHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _appUserRepository.GetByUsernameAsync(request.Username, cancellationToken);

            if (user is null)
                throw new NotFoundException("AppUser", "username", request.Username);

            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                throw new ValidationException("Current password is incorrect.");

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
                throw new ValidationException("New password must be different from the current password.");

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var updated = await _appUserRepository.UpdatePasswordAsync(user.Id, newPasswordHash, cancellationToken);

            if (!updated)
                throw new NotFoundException("AppUser", "username", request.Username);

            return updated;
        }
    }
}
