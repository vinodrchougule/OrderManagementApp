using MediatR;

namespace OrderManagementApp.BLL.Features.AppRoles.Commands
{
    public record UpdateAppRoleCommand(long Id, string RoleName) : IRequest<bool>;
}
