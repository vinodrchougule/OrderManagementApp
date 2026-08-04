using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.DAL.Repositories
{
    public class AppRoleRepository : IAppRoleRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AppRoleRepository> _logger;

        public AppRoleRepository(AppDbContext dbContext, ILogger<AppRoleRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AppRole> CreateAsync(AppRole appRole, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await _dbContext.AppRoles.AddAsync(appRole, ct);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("AppRole id {AppRoleId} created successfully.", appRole.AppRoleId);
                return appRole;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error creating app role.");
                throw;
            }
        }

        public async Task<bool> ExistsByNameAsync(string roleName, long? excludeId = null, CancellationToken ct = default)
        {
            return await _dbContext.AppRoles
                                    .AsNoTracking()
                                    .AnyAsync(r => r.RoleName == roleName && (excludeId == null || r.AppRoleId != excludeId), ct);
        }

        public async Task<List<AppRole>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbContext.AppRoles
                                    .AsNoTracking()
                                    .OrderBy(r => r.RoleName)
                                    .ToListAsync(ct);
        }

        public async Task<AppRole?> GetByIdAsync(long id, CancellationToken ct = default)
        {
            _logger.LogInformation("Fetching app role id {AppRoleId}..", id);

            return await _dbContext.AppRoles
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(r => r.AppRoleId == id, ct);
        }

        public async Task<bool> UpdateAsync(long id, AppRole appRole, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var existingAppRole = await _dbContext.AppRoles.FirstOrDefaultAsync(r => r.AppRoleId == id, ct);

                if (existingAppRole is null)
                    return false;

                existingAppRole.RoleName = appRole.RoleName;

                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("AppRole id {AppRoleId} updated successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error updating app role id {AppRoleId}.", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

            try
            {
                var existingAppRole = await _dbContext.AppRoles.FirstOrDefaultAsync(r => r.AppRoleId == id, ct);

                if (existingAppRole is null)
                    return false;

                _dbContext.AppRoles.Remove(existingAppRole);
                await _dbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync(ct);

                _logger.LogInformation("AppRole id {AppRoleId} deleted successfully.", id);
                return true;
            }
            catch (Exception ex)
            {
                await dbContextTransaction.RollbackAsync(ct);
                _logger.LogError(ex, "Error deleting app role id {AppRoleId}.", id);
                throw;
            }
        }
    }
}
