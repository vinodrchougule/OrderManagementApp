using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Mappers
{
    [Mapper]
    public static partial class ItemMapper
    {
        [MapperIgnoreTarget(nameof(Item.Id))]
        [MapperIgnoreTarget(nameof(Item.OrderItems))]
        public static partial Item ToEntity(CreateItemRequest dto);

        [MapperIgnoreTarget(nameof(Item.OrderItems))]
        public static partial Item ToEntity(UpdateItemRequest dto);

        [MapperIgnoreSource(nameof(Item.OrderItems))]
        public static partial ItemResponse ToResponse(Item item);

        public static partial List<ItemResponse> ToResponseList(List<Item> items);
    }
}
