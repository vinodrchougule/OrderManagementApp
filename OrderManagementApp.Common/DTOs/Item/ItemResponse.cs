using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs
{
    public class ItemResponse
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = string.Empty;
    }
}
