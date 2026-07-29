using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using OrderManagementApp.Common.DTOs.Auth;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.DAL.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<AppUserRepository> _logger;

        public AppUserRepository(AppDbContext appDbContext, ILogger<AppUserRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<AppUser> CreateAsync(AppUser appUser, CancellationToken ct = default)
        {
            await using IDbContextTransaction dbContextTransaction = await _appDbContext.Database.BeginTransactionAsync(ct);

            try
            {
                await _appDbContext.AddAsync(appUser, ct);
                await _appDbContext.SaveChangesAsync(ct);
                await dbContextTransaction.CommitAsync();

                _logger.LogInformation("User created successfully.");
                return appUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user.");
                await dbContextTransaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task<AppUser?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _appDbContext.AppUsers
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<AppUser?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
             return await _appDbContext.AppUsers
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Username == username, ct);
        }

        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _appDbContext.AppUsers
                                     .AsNoTracking()
                                     .FirstOrDefaultAsync(x => x.Email == email, ct);
        }

        public async Task UpdateRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiry, CancellationToken ct = default)
        {
            try
            {
                var user = await _appDbContext.AppUsers
                                        .FirstOrDefaultAsync(u => u.Id == userId, ct);

                if (user is null)
                    return;

                user.RefreshToken       = refreshToken;
                user.RefreshTokenExpiry = expiry;

                await _appDbContext.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating refresh token for user with ID {UserId}.", userId);
                throw;
            }
        }
    }
}
