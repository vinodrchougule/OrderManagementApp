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

        public async Task<bool> ExistsByNameAsync(string customerName, CancellationToken ct = default)
        {
            return await _dbContext.Customers
                                    .AsNoTracking()
                                    .AnyAsync(c => c.CustomerName == customerName, ct);
        }
    }
}
