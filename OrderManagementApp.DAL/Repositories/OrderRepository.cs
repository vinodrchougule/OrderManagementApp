using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OrderManagementApp.Common.Data;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;
using OrderManagementApp.Domain.Models;
using System.Data;

namespace OrderManagementApp.DAL.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext dbContext, IDbConnectionFactory connectionFactory, ILogger<OrderRepository> logger)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<Order> CreateAsync(Order order, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                order.OrderDate = DateTime.UtcNow;
                order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

                await _dbContext.Orders.AddAsync(order);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Order {Orderid} created successfully.", order.OrderId);
                return order;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync();
                _logger.LogError(ex, "Error creating order.");
                throw;
            }
        }

        public async Task<Order?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching order {OrderId}..", id);

            using IDbConnection dbConnection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@OrderId", id, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync(
                sql: "dbo.spGetOrderById",
                param: parameters,
                commandType: CommandType.StoredProcedure);

            var orderResult = multi.Read<Order, Customer, Order>(
                (order, customer) =>
                {
                    order.Customer = customer;
                    order.CustomerId = customer.Id;
                    return order;
                },
                splitOn: "Id");

            var order = orderResult.SingleOrDefault();

            if (order is null)
                return null;

            var itemResult = multi.Read<OrderItem, Item, OrderItem>(
                (orderItem, item) =>
                {
                    orderItem.Item = item;
                    orderItem.ItemId = item.Id;
                    return orderItem;
                },
                splitOn: "Id");

            order.OrderItems = itemResult.ToList();

            return order;

            //return await _dbContext.Orders
            //                        .AsNoTracking()
            //                        .Include(c => c.Customer)
            //                        .Include (oi => oi.OrderItems).ThenInclude(i => i.Item)
            //                        .FirstOrDefaultAsync(o => o.OrderId == id, ct);
        }

        public async Task<PagedResult<Order>> GetAllAsync(int PageNo, int PageSize, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching all orders..");
            using IDbConnection dbConnection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PageNo", PageNo, DbType.Int32);
            parameters.Add("@PageSize", PageSize, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync(
                sql: "dbo.spGetAllOrders",
                param: parameters,
                commandType: CommandType.StoredProcedure);

            var totalCount = await multi.ReadSingleAsync<int>();

            var orders = multi.Read<Order, Customer, Order>(
                (order, customer) =>
                {
                    order.Customer = customer;
                    order.CustomerId = customer.Id;
                    order.OrderItems = new List<OrderItem>();
                    return order;
                },
                splitOn:"Id").ToDictionary(o => o.OrderId);

            var orderItems = multi.Read<OrderItem, Item, OrderItem>(
                (orderItem, item) =>
                {
                    orderItem.Item = item;
                    orderItem.ItemId = item.Id;
                    return orderItem;
                },
                splitOn:"Id");

            foreach (var orderItem in orderItems) 
            {
                if(orders.TryGetValue(orderItem.OrderId, out var order))
                {
                    order.OrderItems.Add(orderItem);
                }
            }

            return new PagedResult<Order>
            {
                Items = orders.Values,
                TotalCount = totalCount,
                PageNo = PageNo,
                PageSize = PageSize
            };

            //return orders.Values;

            //return await _dbContext.Orders
            //                        .AsNoTracking()
            //                        .Include(c => c.Customer)
            //                        .Include(oi => oi.OrderItems).ThenInclude(i => i.Item)
            //                        .OrderByDescending(o => o.OrderDate)
            //                        .ToListAsync(ct);
        }

        public async Task<bool> UpdateAsync(int id, Order incomingOrder, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                if (incomingOrder is null)
                    return false;

                if (id != incomingOrder.OrderId)
                    return false;

                var existingOrder = await _dbContext.Orders
                                              .Include(oi => oi.OrderItems)
                                              .FirstOrDefaultAsync(o => o.OrderId == id, ct);

                if (existingOrder is null)
                    return false;

                _dbContext.Entry(existingOrder).CurrentValues.SetValues(incomingOrder);
                _dbContext.Entry(existingOrder).Property(o => o.RowVersion).OriginalValue = incomingOrder.RowVersion;

                var incomingIds = incomingOrder.OrderItems
                                               .Where(oi => oi.OrderItemId != 0)
                                               .Select(oi => oi.OrderItemId)
                                               .ToHashSet();

                var orderItemIdsToRemove = existingOrder.OrderItems
                                                   .Where(oi => !incomingIds.Contains(oi.OrderItemId))
                                                   .ToList();

                foreach (var itemId in orderItemIdsToRemove)
                {
                    existingOrder.OrderItems.Remove(itemId);
                    _dbContext.OrderItems.Remove(itemId);
                }

                foreach (var incomingOrderItemId in incomingOrder.OrderItems)
                {
                    var existingItem = existingOrder.OrderItems
                                                    .FirstOrDefault(oi => oi.OrderItemId == incomingOrderItemId.OrderItemId && 
                                                        incomingOrderItemId.OrderItemId != 0);

                    if (existingItem is not null)
                    {
                        _dbContext.Entry(existingItem).CurrentValues.SetValues(incomingOrderItemId);
                        existingItem.OrderId = existingOrder.OrderId;
                    }
                    else
                    {
                        incomingOrderItemId.OrderItemId = 0;
                        incomingOrderItemId.OrderId = existingOrder.OrderId;
                        existingOrder.OrderItems.Add(incomingOrderItemId);
                    }
                }

                existingOrder.TotalAmount = incomingOrder.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Order {OrderId} updated successfully", id);
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError("Database update conflict error occurred. error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError("Error occurred. error: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int OrderId, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var order = await _dbContext.Orders
                                              .Include(oi => oi.OrderItems)
                                              .FirstOrDefaultAsync(o => o.OrderId == OrderId, ct);

                if (order is null)
                    return false;

                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("Order {OrderId} deleted successfully.", OrderId);
                return true;
            }
            catch(Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError("Error occurred while deleting order {OrderId}. error: {Message}", OrderId, ex.Message);
                throw;
            }
        }
    }
}
