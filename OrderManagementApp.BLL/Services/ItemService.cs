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
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly CreateItemRequestValidator _createItemRequestValidator;
        private readonly UpdateItemRequestValidator _updateItemRequestValidator;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
            _createItemRequestValidator = new CreateItemRequestValidator();
            _updateItemRequestValidator = new UpdateItemRequestValidator();
        }

        public async Task<ItemResponse> CreateAsync(CreateItemRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _createItemRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _itemRepository.ExistsByNameAsync(dto.ItemName, ct: ct);

            if (nameExists)
                throw new ValidationException("Item Name already exists.");

            var item = ItemMapper.ToEntity(dto);

            var created = await _itemRepository.CreateAsync(item, ct);

            return ItemMapper.ToResponse(created);
        }

        public async Task<List<ItemResponse>> GetAllAsync(CancellationToken ct = default)
        {
            var items = await _itemRepository.GetAllAsync(ct);

            return ItemMapper.ToResponseList(items);
        }

        public async Task<ItemResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var item = await _itemRepository.GetByIdAsync(id, ct);

            if (item is null)
                throw new NotFoundException("Item", "id", id);

            return ItemMapper.ToResponse(item);
        }

        public async Task<bool> UpdateAsync(int id, UpdateItemRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _updateItemRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var nameExists = await _itemRepository.ExistsByNameAsync(dto.ItemName, dto.Id, ct);

            if (nameExists)
                throw new ValidationException("Item Name already exists.");

            var item = ItemMapper.ToEntity(dto);

            var updated = await _itemRepository.UpdateAsync(id, item, ct);

            if (!updated)
                throw new NotFoundException("Item", "id", id);

            return updated;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var hasOrderItems = await _itemRepository.HasOrderItemsAsync(id, ct);

            if (hasOrderItems)
                throw new ValidationException("Cannot delete item because it is referenced by existing orders.");

            var deleted = await _itemRepository.DeleteAsync(id, ct);

            if (!deleted)
                throw new NotFoundException("Item", "id", id);

            return deleted;
        }
    }
}
