using MediatR;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public record ForgotPasswordCommand(string Email) : IRequest<bool>;
}
