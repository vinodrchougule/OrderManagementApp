using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs.Auth
{
    public class RegisterUserRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
    }
}
