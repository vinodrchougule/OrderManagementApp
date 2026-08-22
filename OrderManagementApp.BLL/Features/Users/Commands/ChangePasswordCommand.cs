using MediatR;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public record ChangePasswordCommand(string Username, string CurrentPassword, string NewPassword) : IRequest<bool>;
}
