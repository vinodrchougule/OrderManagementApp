using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
