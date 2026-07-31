using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest dto, CancellationToken ct = default);
        Task<List<CustomerResponse>> GetAllAsync(CancellationToken ct = default);
        Task<CustomerResponse> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateCustomerRequest dto, CancellationToken ct = default);
    }
}
