using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}
