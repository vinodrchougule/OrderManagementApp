using Dapper;
using OrderManagementApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrderManagementApp.Common.Data
{
    public class OrderStatusTypeHandler : SqlMapper.TypeHandler<OrderStatus>
    {
        public override void SetValue(IDbDataParameter parameter, OrderStatus value)
        {
            parameter.Value = value.ToString();
        }

        public override OrderStatus Parse(object value)
        {
            return Enum.Parse<OrderStatus>((string) value, ignoreCase: true);
        }
    }
}
