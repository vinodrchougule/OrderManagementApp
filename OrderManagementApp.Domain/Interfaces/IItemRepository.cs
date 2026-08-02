using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface IItemRepository
    {
        Task<Item> CreateAsync(Item item, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string itemName, int? excludeId = null, CancellationToken ct = default);
        Task<List<Item>> GetAllAsync(CancellationToken ct = default);
        Task<Item?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, Item item, CancellationToken ct = default);
        Task<bool> HasOrderItemsAsync(int itemId, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
