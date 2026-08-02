using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Interfaces
{
    public interface IItemService
    {
        Task<ItemResponse> CreateAsync(CreateItemRequest dto, CancellationToken ct = default);
        Task<List<ItemResponse>> GetAllAsync(CancellationToken ct = default);
        Task<ItemResponse> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, UpdateItemRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
