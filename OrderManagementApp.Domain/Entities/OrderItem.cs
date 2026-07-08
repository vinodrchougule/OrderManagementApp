using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Entities
{
    public class OrderItem
    {
        public long OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Order? Order { get; set; }
        public Item? Item { get; set; }
    }
}
