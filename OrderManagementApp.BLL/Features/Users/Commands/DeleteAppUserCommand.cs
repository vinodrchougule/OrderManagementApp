using MediatR;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public record DeleteAppUserCommand(int Id) : IRequest<bool>;
}
