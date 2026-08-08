using MediatR;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.BLL.Features.AppRoles.Queries
{
    public record GetAppRoleByIdQuery(long Id) : IRequest<AppRoleResponse>;
}
