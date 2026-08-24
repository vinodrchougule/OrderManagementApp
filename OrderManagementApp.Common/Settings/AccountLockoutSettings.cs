namespace OrderManagementApp.Common.Settings
{
    public class AccountLockoutSettings
    {
        public int MaxFailedAttempts { get; set; } = 3;
        public int LockoutDurationMinutes { get; set; } = 15;
    }
}
