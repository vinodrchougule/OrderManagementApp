using OrderManagementApp.Domain.Models;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order, CancellationToken ct = default);
        Task<Order?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<Order>> GetAllAsync(int PageNo, int PageSize, CancellationToken ct = default);
        Task<PagedResult<Order>> SearchAsync(string searchText, int PageNo, int PageSize, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, Order incomingOrder, CancellationToken ct = default);
        Task<bool> DeleteAsync(int OrderId, CancellationToken ct = default);
    }
}
