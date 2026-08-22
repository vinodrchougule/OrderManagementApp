using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        public string? Username { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
