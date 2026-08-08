using FluentValidation;
using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.AppRoles.Commands
{
    public class UpdateAppRoleCommandHandler : IRequestHandler<UpdateAppRoleCommand, bool>
    {
        private readonly IAppRoleRepository _appRoleRepository;

        public UpdateAppRoleCommandHandler(IAppRoleRepository appRoleRepository)
        {
            _appRoleRepository = appRoleRepository;
        }

        public async Task<bool> Handle(UpdateAppRoleCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _appRoleRepository.ExistsByNameAsync(request.RoleName, request.Id, cancellationToken);

            if (nameExists)
                throw new ValidationException("Role Name already exists.");

            var appRole = AppRoleMapper.ToEntity(request);

            var updated = await _appRoleRepository.UpdateAsync(request.Id, appRole, cancellationToken);

            if (!updated)
                throw new NotFoundException("AppRole", "id", request.Id);

            return updated;
        }
    }
}
