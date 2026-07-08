using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs.Auth
{
    public class AuthResponse
    {
        public int id {  get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime AccessTokenExpiry { get; set; }

    }
}
