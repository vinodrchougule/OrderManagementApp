using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.Settings
{
    public class JwtSettings
    {
        public string? SecretKey { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int AccessTokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }
}
