using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.Domain.Interfaces
{
    public interface IAppRoleRepository
    {
        Task<AppRole> CreateAsync(AppRole appRole, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string roleName, long? excludeId = null, CancellationToken ct = default);
        Task<List<AppRole>> GetAllAsync(CancellationToken ct = default);
        Task<AppRole?> GetByIdAsync(long id, CancellationToken ct = default);
        Task<bool> UpdateAsync(long id, AppRole appRole, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    }
}
