using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface IAppUserRepository
    {
        Task<AppUser> CreateAsync(AppUser appUser, CancellationToken ct = default);
        Task<bool> ExistsByUsernameAsync(string username, int? excludeId = null, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, int? excludeId = null, CancellationToken ct = default);
        Task<List<AppUser>> GetAllAsync(CancellationToken ct = default);
        Task<AppUser?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AppUser?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> UpdateAsync(int id, AppUser appUser, CancellationToken ct = default);
        Task UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiry, CancellationToken ct = default);
        Task<bool> UpdatePasswordAsync(int userId, string newPasswordHash, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsInAuditLogAsync(string username, CancellationToken ct = default);
    }
}
