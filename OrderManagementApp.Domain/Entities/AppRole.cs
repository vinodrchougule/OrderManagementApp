using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Entities
{
    public class AppRole
    {
        public long AppRoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
