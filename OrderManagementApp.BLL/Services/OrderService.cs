using FluentValidation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.BLL.Validators;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;
using OrderManagementApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly CreateOrderRequestValidator _createOrderRequestValidator;
        private readonly UpdateOrderRequestValidator _updateOrderRequestValidator;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
            _createOrderRequestValidator = new CreateOrderRequestValidator();
            _updateOrderRequestValidator = new UpdateOrderRequestValidator();
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _createOrderRequestValidator.ValidateAsync(dto, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var order = OrderMapper.ToEntity(dto);

            var created = await _orderRepository.CreateAsync(order, ct);

            var reloaded = await _orderRepository.GetByIdAsync(created.OrderId, ct) ?? throw new NotFoundException(nameof(order), created.OrderId);

            return OrderMapper.ToResponse(reloaded);
        }

        public async Task<OrderResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var order = await _orderRepository.GetByIdAsync(id, ct);

            if(order is null)
                throw new NotFoundException(nameof(order), id);

            return OrderMapper.ToResponse(order);
        }

        public async Task<PagedResult<OrderResponse>> GetAllAsync(int pageNo, int pageSize,CancellationToken ct = default)
        {
            var orders = await _orderRepository.GetAllAsync(pageNo, pageSize,ct);

            var orderResponseDTOs = orders.Items.Select(order => new OrderResponse
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.CustomerName,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                RowVersion = order.RowVersion,
                OrderItems = order.OrderItems.Select(oi => new OrderItemResponse 
                {
                    OrderItemId = oi.OrderItemId,
                    ItemId = oi.ItemId,
                    ItemName = oi.Item?.ItemName,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    LineTotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            }).ToList();

            return new PagedResult<OrderResponse>
            {
                Items = orderResponseDTOs,
                TotalCount = orders.TotalCount,
                PageNo = orders.PageNo,
                PageSize = orders.PageSize
            };

            //return orders.Select(OrderMapper.ToResponse).ToList();

        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderRequest dto, CancellationToken ct = default)
        {
            var validationResult = await _updateOrderRequestValidator.ValidateAsync(dto, ct);

            if(!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            try
            {
                var order = OrderMapper.ToEntity(dto);

                var updated = await _orderRepository.UpdateAsync(id, order, ct);

                if (!updated)
                    throw new NotFoundException(nameof(order), id);

                return updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConcurrencyException();
            }
        }

        public async Task<bool> DeleteAsync(int OrderId, CancellationToken ct = default)
        {
            var deleted = await _orderRepository.DeleteAsync(OrderId, ct);

            if(!deleted)
                throw new NotFoundException(nameof(Order),OrderId);

            return deleted;
        }
    }
}
