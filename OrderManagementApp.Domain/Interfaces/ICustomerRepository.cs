using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string customerName, int? excludeId = null, CancellationToken ct = default);
        Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
        Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, Customer customer, CancellationToken ct = default);
    }
}
