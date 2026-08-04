using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs
{
    public class OrderItemResponse
    {
        public long OrderItemId { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
