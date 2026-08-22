using MediatR;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.BLL.Features.Users.Queries
{
    public record GetAllAppUsersQuery : IRequest<List<AppUserResponse>>;
}
