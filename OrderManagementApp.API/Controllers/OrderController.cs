using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementApp.API.Middleware;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Services;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderRequest createOrderRequest, CancellationToken ct)
        {
            var order = await _orderService.CreateAsync(createOrderRequest, ct);
            return Ok("Created. Order Id:" + order.OrderId);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderResponse>> GetById(int id, CancellationToken ct)
        {
            try
            {
                var order = await _orderService.GetByIdAsync(id, ct);

                if(order is null)
                    return BadRequest("Order not found.");

                return Ok(order);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetAll([FromQuery] PagedRequest pagedRequest,CancellationToken ct)
        {
            if (pagedRequest.PageNo < 1)
                return BadRequest("Invalid Page No.");

            var orders= await _orderService.GetAllAsync(pagedRequest.PageNo, pagedRequest.PageSize, ct);
            return Ok(orders);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateOrderRequest updateOrderRequest, CancellationToken ct)
        {
            if (id != updateOrderRequest.OrderId)
                return BadRequest("Route Order Id and Request body Order Id do not match");

            var updated = await _orderService.UpdateAsync(id, updateOrderRequest, ct);

            if (!updated)
                return BadRequest("Update failed.");

            return Ok("Updated");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _orderService.DeleteAsync(id,ct);

            if (!deleted)
                return BadRequest("Delete failed.");

            return Ok("Deleted");
        }
    }
}
