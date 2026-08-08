using MediatR;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.BLL.Features.AppRoles.Commands
{
    public record CreateAppRoleCommand(string RoleName) : IRequest<AppRoleResponse>;
}
