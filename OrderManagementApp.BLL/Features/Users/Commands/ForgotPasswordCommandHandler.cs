using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.Settings;
using OrderManagementApp.Domain.Interfaces;
using System.Security.Cryptography;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IEmailService _emailService;
        private readonly PasswordResetSettings _passwordResetSettings;
        private readonly ILogger<ForgotPasswordCommandHandler> _logger;

        public ForgotPasswordCommandHandler(
            IAppUserRepository appUserRepository,
            IEmailService emailService,
            IOptions<PasswordResetSettings> passwordResetSettings,
            ILogger<ForgotPasswordCommandHandler> logger)
        {
            _appUserRepository = appUserRepository;
            _emailService = emailService;
            _passwordResetSettings = passwordResetSettings.Value;
            _logger = logger;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _appUserRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Forgot password requested for an email that does not exist: {Email}.", request.Email);
                return true;
            }

            var resetToken = GenerateResetToken();
            var expiry = DateTime.UtcNow.AddMinutes(_passwordResetSettings.TokenExpiryMinutes);

            await _appUserRepository.SetPasswordResetTokenAsync(user.Id, resetToken, expiry, cancellationToken);

            var resetLink = $"{_passwordResetSettings.ClientResetPasswordUrl}?token={Uri.EscapeDataString(resetToken)}";

            var htmlBody = $@"
                <p>Hello {user.Username},</p>
                <p>We received a request to reset your password. Click the link below to choose a new password:</p>
                <p><a href=""{resetLink}"">Reset your password</a></p>
                <p>This link will expire in {_passwordResetSettings.TokenExpiryMinutes} minutes. If you did not request this, you can safely ignore this email.</p>";

            await _emailService.SendEmailAsync(user.Email!, "Reset your password", htmlBody, cancellationToken);

            return true;
        }

        private static string GenerateResetToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(randomBytes)
                           .Replace("+", "-")
                           .Replace("/", "_")
                           .Replace("=", "");
        }
    }
}
