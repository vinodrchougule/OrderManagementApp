using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ILogger<ItemController> _logger;

        public ItemController(IItemService itemService, ILogger<ItemController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<ItemResponse>> Create([FromBody] CreateItemRequest createItemRequest, CancellationToken ct)
        {
            var item = await _itemService.CreateAsync(createItemRequest, ct);
            return Ok(item);
        }

        [HttpGet]
        public async Task<ActionResult<List<ItemResponse>>> GetAll(CancellationToken ct)
        {
            var items = await _itemService.GetAllAsync(ct);
            return Ok(items);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ItemResponse>> GetById(int id, CancellationToken ct)
        {
            var item = await _itemService.GetByIdAsync(id, ct);
            return Ok(item);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateItemRequest updateItemRequest, CancellationToken ct)
        {
            if (id != updateItemRequest.Id)
                return BadRequest("Route Id and Request body Id do not match");

            var updated = await _itemService.UpdateAsync(id, updateItemRequest, ct);

            if (!updated)
                return BadRequest("Update failed.");

            return Ok("Updated");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _itemService.DeleteAsync(id, ct);

            if (!deleted)
                return BadRequest("Delete failed.");

            return Ok("Deleted");
        }
    }
}
