using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface IAppUserRepository
    {
        Task<AppUser> CreateAsync(AppUser appUser, CancellationToken ct = default);
        Task<AppUser?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AppUser?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiry, CancellationToken ct = default);
    }
}
