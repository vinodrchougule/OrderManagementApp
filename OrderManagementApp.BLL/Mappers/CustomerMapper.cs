using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Mappers
{
    [Mapper]
    public static partial class CustomerMapper
    {
        [MapperIgnoreTarget(nameof(Customer.Id))]
        [MapperIgnoreTarget(nameof(Customer.Orders))]
        public static partial Customer ToEntity(CreateCustomerRequest dto);

        [MapperIgnoreSource(nameof(Customer.Orders))]
        public static partial CustomerResponse ToResponse(Customer customer);
    }
}
