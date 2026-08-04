using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs
{
    public class UpdateAppRoleRequest
    {
        public long Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
