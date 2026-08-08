using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.AppRoles.Queries
{
    public class GetAppRoleByIdQueryHandler : IRequestHandler<GetAppRoleByIdQuery, AppRoleResponse>
    {
        private readonly IAppRoleRepository _appRoleRepository;

        public GetAppRoleByIdQueryHandler(IAppRoleRepository appRoleRepository)
        {
            _appRoleRepository = appRoleRepository;
        }

        public async Task<AppRoleResponse> Handle(GetAppRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var appRole = await _appRoleRepository.GetByIdAsync(request.Id, cancellationToken);

            if (appRole is null)
                throw new NotFoundException("AppRole", "id", request.Id);

            return AppRoleMapper.ToResponse(appRole);
        }
    }
}
