using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.DAL.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(AppDbContext dbContext, ILogger<CustomerRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Customer> CreateAsync(Customer customer, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await _dbContext.Customers.AddAsync(customer, ct);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Customer {CustomerId} created successfully.", customer.Id);
                return customer;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error creating customer.");
                throw;
            }
        }

        public async Task<bool> ExistsByNameAsync(string customerName, int? excludeId = null, CancellationToken ct = default)
        {
            return await _dbContext.Customers
                                    .AsNoTracking()
                                    .AnyAsync(c => c.CustomerName == customerName && (excludeId == null || c.Id != excludeId), ct);
        }

        public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.Customers
                                    .AsNoTracking()
                                    .OrderBy(c => c.CustomerName)
                                    .ToListAsync(ct);
        }

        public async Task<Customer?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching customer {CustomerId}..", id);

            return await _dbContext.Customers
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<bool> UpdateAsync(int id, Customer customer, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var existingCustomer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

                if (existingCustomer is null)
                    return false;

                existingCustomer.CustomerName = customer.CustomerName;

                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Customer id {CustomerId} updated successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error updating customer id {CustomerId}.", id);
                throw;
            }
        }
    }
}
