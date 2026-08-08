using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.AppRoles.Queries
{
    public class GetAllAppRolesQueryHandler : IRequestHandler<GetAllAppRolesQuery, List<AppRoleResponse>>
    {
        private readonly IAppRoleRepository _appRoleRepository;

        public GetAllAppRolesQueryHandler(IAppRoleRepository appRoleRepository)
        {
            _appRoleRepository = appRoleRepository;
        }

        public async Task<List<AppRoleResponse>> Handle(GetAllAppRolesQuery request, CancellationToken cancellationToken)
        {
            var appRoles = await _appRoleRepository.GetAllAsync(cancellationToken);

            return AppRoleMapper.ToResponseList(appRoles);
        }
    }
}
