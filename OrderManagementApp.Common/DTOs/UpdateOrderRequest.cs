using OrderManagementApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Common.DTOs
{
    public class UpdateOrderRequest
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public int CustomerId { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public List<UpdateOrderItemRequest> OrderItems { get; set; } = new();
    }
}
