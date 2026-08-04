using FluentValidation;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.BLL.Validators;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Services
{
    public class AppRoleService : IAppRoleService
    {
        private readonly IAppRoleRepository _appRoleRepository;
        private readonly CreateAppRoleRequestValidator _createAppRoleRequestValidator;
        private readonly UpdateAppRoleRequestValidator _updateAppRoleRequestValidator;

        public AppRoleService(IAppRoleRepository appRoleRepository)
        {
            _appRoleRepository = appRoleRepository;
            _createAppRoleRequestValidator = new CreateAppRoleRequestValidator();
            _updateAppRoleRequestValidator = new UpdateAppRoleRequestValidator();
        }

        public async Task<AppRoleResponse> CreateAsync(CreateAppRoleRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _createAppRoleRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _appRoleRepository.ExistsByNameAsync(dto.RoleName, ct: ct);

            if (nameExists)
                throw new ValidationException("Role Name already exists.");

            var appRole = AppRoleMapper.ToEntity(dto);

            var created = await _appRoleRepository.CreateAsync(appRole, ct);

            return AppRoleMapper.ToResponse(created);
        }

        public async Task<List<AppRoleResponse>> GetAllAsync(CancellationToken ct = default)
        {
            var appRoles = await _appRoleRepository.GetAllAsync(ct);

            return AppRoleMapper.ToResponseList(appRoles);
        }

        public async Task<AppRoleResponse> GetByIdAsync(long id, CancellationToken ct = default)
        {
            var appRole = await _appRoleRepository.GetByIdAsync(id, ct);

            if (appRole is null)
                throw new NotFoundException("AppRole", "id", id);

            return AppRoleMapper.ToResponse(appRole);
        }

        public async Task<bool> UpdateAsync(long id, UpdateAppRoleRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _updateAppRoleRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _appRoleRepository.ExistsByNameAsync(dto.RoleName, dto.Id, ct);

            if (nameExists)
                throw new ValidationException("Role Name already exists.");

            var appRole = AppRoleMapper.ToEntity(dto);

            var updated = await _appRoleRepository.UpdateAsync(id, appRole, ct);

            if (!updated)
                throw new NotFoundException("AppRole", "id", id);

            return updated;
        }

        public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
        {
            var deleted = await _appRoleRepository.DeleteAsync(id, ct);

            if (!deleted)
                throw new NotFoundException("AppRole", "id", id);

            return deleted;
        }
    }
}
