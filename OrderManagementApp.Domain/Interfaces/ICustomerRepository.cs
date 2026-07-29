using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string customerName, CancellationToken ct = default);
    }
}
