using OrderManagementApp.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Interfaces
{
    public interface IAppRoleService
    {
        Task<AppRoleResponse> CreateAsync(CreateAppRoleRequest dto, CancellationToken ct = default);
        Task<List<AppRoleResponse>> GetAllAsync(CancellationToken ct = default);
        Task<AppRoleResponse> GetByIdAsync(long id, CancellationToken ct = default);
        Task<bool> UpdateAsync(long id, UpdateAppRoleRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    }
}
