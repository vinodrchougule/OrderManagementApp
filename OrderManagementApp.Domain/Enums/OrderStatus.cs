using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        Confirmed,
        Shipped,
        Delivered,
        Cancelled
    }
}
