using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.DAL.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ItemRepository> _logger;

        public ItemRepository(AppDbContext dbContext, ILogger<ItemRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Item> CreateAsync(Item item, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await _dbContext.Items.AddAsync(item, ct);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Item id {ItemId} created successfully.", item.Id);
                return item;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error creating item.");
                throw;
            }
        }

        public async Task<bool> ExistsByNameAsync(string itemName, int? excludeId = null, CancellationToken ct = default)
        {
            return await _dbContext.Items
                                    .AsNoTracking()
                                    .AnyAsync(i => i.ItemName == itemName && (excludeId == null || i.Id != excludeId), ct);
        }

        public async Task<List<Item>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.Items
                                    .AsNoTracking()
                                    .OrderBy(i => i.ItemName)
                                    .ToListAsync(ct);
        }

        public async Task<Item?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching item id {ItemId}..", id);

            return await _dbContext.Items
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(i => i.Id == id, ct);
        }

        public async Task<bool> UpdateAsync(int id, Item item, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var existingItem = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, ct);

                if (existingItem is null)
                    return false;

                existingItem.ItemName = item.ItemName;

                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Item id {ItemId} updated successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error updating item id {ItemId}.", id);
                throw;
            }
        }

        public async Task<bool> HasOrderItemsAsync(int itemId, CancellationToken ct = default)
        {
            return await _dbContext.OrderItems
                                    .AsNoTracking()
                                    .AnyAsync(oi => oi.ItemId == itemId, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var existingItem = await _dbContext.Items.FirstOrDefaultAsync(i => i.Id == id, ct);

                if (existingItem is null)
                    return false;

                _dbContext.Items.Remove(existingItem);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Item id {ItemId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error deleting item id {ItemId}.", id);
                throw;
            }
        }
    }
}
