namespace OrderManagementApp.Common.Settings
{
    public class PasswordResetSettings
    {
        public string? ClientResetPasswordUrl { get; set; }
        public int TokenExpiryMinutes { get; set; } = 30;
    }
}
