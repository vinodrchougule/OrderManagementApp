// Lives in OrderManagementApp.API project, e.g. Services/GrpcOrderService.cs
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using OrderManagementApp.API.Protos; // wherever your csharp_namespace points
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Services;
using OrderManagementApp.Common.DTOs; // your existing BLL service interface

namespace OrderManagementApp.API.Services
{
    [Authorize]
    public class GrpcOrderService : OrderGrpcService.OrderGrpcServiceBase
    {
        private readonly IOrderService _orderService;

        public GrpcOrderService(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public override async Task<OrderReply> GetOrderById(
            GetOrderRequest request, ServerCallContext context)
        {
            var userId = context.GetHttpContext().User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            var order = await _orderService.GetByIdAsync(request.OrderId, context.CancellationToken) ?? throw new RpcException(new Status(StatusCode.NotFound,
                    $"Order with id {request.OrderId} was not found."));

            // map order -> OrderReply, return it
            var reply = new OrderReply
            {
                OrderId = order.OrderId,
                OrderDate = Timestamp.FromDateTime(DateTime.SpecifyKind(order.OrderDate, DateTimeKind.Utc)),
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName ?? string.Empty,
                TotalAmount = order.TotalAmount.ToString(),
                Status = (OrderStatus)(int)order.Status, // verify enum numeric values match proto
                RowVersion = Google.Protobuf.ByteString.CopyFrom(order.RowVersion)
            };

            foreach (var item in order.OrderItems)
            {
                reply.OrderItems.Add(new OrderItem
                {
                    OrderItemId = Convert.ToInt32(item.OrderItemId),
                    ItemId = item.ItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice.ToString(),
                });
            }

            return reply;
        }
    }
}