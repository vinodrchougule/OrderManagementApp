using FluentValidation;
using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.AppRoles.Commands
{
    public class CreateAppRoleCommandHandler : IRequestHandler<CreateAppRoleCommand, AppRoleResponse>
    {
        private readonly IAppRoleRepository _appRoleRepository;

        public CreateAppRoleCommandHandler(IAppRoleRepository appRoleRepository)
        {
            _appRoleRepository = appRoleRepository;
        }

        public async Task<AppRoleResponse> Handle(CreateAppRoleCommand request, CancellationToken cancellationToken)
        {
            var nameExists = await _appRoleRepository.ExistsByNameAsync(request.RoleName, ct: cancellationToken);

            if (nameExists)
                throw new ValidationException("Role Name already exists.");

            var appRole = AppRoleMapper.ToEntity(request);

            var created = await _appRoleRepository.CreateAsync(appRole, cancellationToken);

            return AppRoleMapper.ToResponse(created);
        }
    }
}
