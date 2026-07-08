using OrderManagementApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public byte[] RowVersion { get; set; } =Array.Empty<byte>();
        
        public Customer? Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
