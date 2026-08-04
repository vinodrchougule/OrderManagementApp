using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs
{
    public class CreateOrderRequest
    {
        public int CustomerId { get; set; }
        public List<CreateOrderItemRequest> OrderItems { get; set; } = new();
    }
}
