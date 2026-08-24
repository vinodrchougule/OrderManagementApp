using MediatR;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public record ResetPasswordCommand(string Token, string NewPassword) : IRequest<bool>;
}
