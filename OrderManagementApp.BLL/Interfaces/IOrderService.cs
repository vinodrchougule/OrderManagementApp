using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateAsync(CreateOrderRequest dto, CancellationToken ct = default);
        Task<OrderResponse> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PagedResult<OrderResponse>> GetAllAsync(int pageNo, int pageSize, CancellationToken ct = default);
        Task<PagedResult<OrderResponse>> SearchAsync(string searchText, int pageNo, int pageSize, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateOrderRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int OrderId, CancellationToken ct = default);
    }
}
