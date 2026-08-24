namespace OrderManagementApp.Common.Settings
{
    public class SmtpSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public bool UseSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? SenderName { get; set; }
        public string? SenderEmail { get; set; }
    }
}
