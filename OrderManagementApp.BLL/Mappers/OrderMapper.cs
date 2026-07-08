using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Mappers
{
    [Mapper]
    public static partial class OrderMapper
    {
        //public static Order ToEntity(CreateOrderRequest dto)
        //{
        //    return new Order
        //    {
        //        CustomerId = dto.CustomerId,
        //        OrderItems = dto.OrderItems.Select(oi => new OrderItem
        //        {
        //            ItemId = oi.ItemId,
        //            Quantity = oi.Quantity,
        //            UnitPrice = oi.UnitPrice
        //        }).ToList()
        //    };
        //}

        [MapperIgnoreTarget(nameof(Order.OrderId))]
        [MapperIgnoreTarget(nameof(Order.OrderDate))]
        [MapperIgnoreTarget(nameof(Order.TotalAmount))]
        [MapperIgnoreTarget(nameof(Order.Status))]
        [MapperIgnoreTarget(nameof(Order.RowVersion))]
        [MapperIgnoreTarget(nameof(Order.Customer))]
        public static partial Order ToEntity(CreateOrderRequest dto);

        [MapperIgnoreTarget(nameof(OrderItem.OrderItemId))]
        [MapperIgnoreTarget(nameof(OrderItem.OrderId))]
        [MapperIgnoreTarget(nameof(OrderItem.Order))]
        [MapperIgnoreTarget(nameof(OrderItem.Item))]
        public static partial OrderItem ToOrderItemEntity(CreateOrderItemRequest dto);

        //public static Order ToEntity(UpdateOrderRequest dto)
        //{
        //    return new Order
        //    {
        //        OrderId = dto.OrderId,
        //        OrderDate = dto.OrderDate,
        //        CustomerId = dto.CustomerId,
        //        RowVersion = dto.RowVersion,
        //        Status = dto.Status,
        //        OrderItems = dto.OrderItems.Select(oi => new OrderItem
        //        {
        //            OrderItemId = oi.OrderItemId,
        //            ItemId = oi.ItemId,
        //            Quantity = oi.Quantity,
        //            UnitPrice = oi.UnitPrice
        //        }).ToList()
        //    };
        //}

        [MapperIgnoreTarget(nameof(Order.TotalAmount))]
        [MapperIgnoreTarget(nameof(Order.Customer))]
        public static partial Order ToEntity(UpdateOrderRequest dto);

        [MapperIgnoreTarget(nameof(OrderItem.OrderId))]
        [MapperIgnoreTarget(nameof(OrderItem.Order))]
        [MapperIgnoreTarget(nameof(OrderItem.Item))]
        public static partial OrderItem ToOrderItemEntity(UpdateOrderItemRequest dto);

        public static OrderResponse ToResponse(Order order)
        {
            var response = ToResponseCore(order);

            foreach (var (item, itemResponse) in order.OrderItems.Zip(response.OrderItems))
            {
                itemResponse.LineTotal = item.Quantity * item.UnitPrice;
            }

            return response;
            //return new OrderResponse
            //{
            //    OrderId = order.OrderId,
            //    OrderDate = order.OrderDate,
            //    CustomerId = order.CustomerId,
            //    CustomerName = order.Customer?.CustomerName,
            //    TotalAmount = order.TotalAmount,
            //    RowVersion = order.RowVersion,
            //    Status = order.Status,
            //    OrderItems = order.OrderItems.Select(oi => new OrderItemResponse
            //    {
            //        OrderItemId = oi.OrderItemId,
            //        ItemId = oi.ItemId,
            //        ItemName = oi.Item?.ItemName,
            //        Quantity = oi.Quantity,
            //        UnitPrice = oi.UnitPrice,
            //        LineTotal = oi.Quantity * oi.UnitPrice
            //    }).ToList()
            //};
        }

        [MapProperty(nameof(Order.Customer) + "." + nameof(Customer.CustomerName), nameof(OrderResponse.CustomerName))]
        private static partial OrderResponse ToResponseCore(Order order);

        // Used automatically by Mapperly when mapping Order.OrderItems -> OrderResponse.OrderItems
        [MapProperty(nameof(OrderItem.Item) + "." + nameof(Item.ItemName), nameof(OrderItemResponse.ItemName))]
        [MapperIgnoreTarget(nameof(OrderItemResponse.LineTotal))]
        [MapperIgnoreSource(nameof(OrderItem.OrderId))]
        [MapperIgnoreSource(nameof(OrderItem.Order))]
        private static partial OrderItemResponse ToItemResponse(OrderItem orderItem);
    }
}
