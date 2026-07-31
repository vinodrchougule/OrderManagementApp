using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs
{
    public class UpdateCustomerRequest
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}
