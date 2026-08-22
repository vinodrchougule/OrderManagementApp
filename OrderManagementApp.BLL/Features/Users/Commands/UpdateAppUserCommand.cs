using MediatR;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public record UpdateAppUserCommand(int Id, string Username, string Email, string Role) : IRequest<bool>;
}
