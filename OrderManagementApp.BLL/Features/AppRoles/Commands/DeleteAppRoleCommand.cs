using MediatR;

namespace OrderManagementApp.BLL.Features.AppRoles.Commands
{
    public record DeleteAppRoleCommand(long Id) : IRequest<bool>;
}
