using FluentValidation;
using MediatR;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public class DeleteAppUserCommandHandler : IRequestHandler<DeleteAppUserCommand, bool>
    {
        private readonly IAppUserRepository _appUserRepository;

        public DeleteAppUserCommandHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<bool> Handle(DeleteAppUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _appUserRepository.GetByIdAsync(request.Id, cancellationToken);

            if (user is null)
                throw new NotFoundException("AppUser", "id", request.Id);

            var existsInAuditLog = await _appUserRepository.ExistsInAuditLogAsync(user.Username ?? string.Empty, cancellationToken);

            if (existsInAuditLog)
                throw new ValidationException("User exists in Audit Log. Cannot delete!");

            var deleted = await _appUserRepository.DeleteAsync(request.Id, cancellationToken);

            if (!deleted)
                throw new NotFoundException("AppUser", "id", request.Id);

            return deleted;
        }
    }
}
