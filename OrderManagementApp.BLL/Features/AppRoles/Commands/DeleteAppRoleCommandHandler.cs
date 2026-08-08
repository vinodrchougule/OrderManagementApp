using MediatR;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.AppRoles.Commands
{
    public class DeleteAppRoleCommandHandler : IRequestHandler<DeleteAppRoleCommand, bool>
    {
        private readonly IAppRoleRepository _appRoleRepository;

        public DeleteAppRoleCommandHandler(IAppRoleRepository appRoleRepository)
        {
            _appRoleRepository = appRoleRepository;
        }

        public async Task<bool> Handle(DeleteAppRoleCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _appRoleRepository.DeleteAsync(request.Id, cancellationToken);

            if (!deleted)
                throw new NotFoundException("AppRole", "id", request.Id);

            return deleted;
        }
    }
}
