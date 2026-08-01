using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest createCustomerRequest, CancellationToken ct)
        {
            var customer = await _customerService.CreateAsync(createCustomerRequest, ct);
            return Ok(customer);
        }

        [HttpGet]
        public async Task<ActionResult<List<CustomerResponse>>> GetAll(CancellationToken ct)
        {
            var customers = await _customerService.GetAllAsync(ct);
            return Ok(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomerResponse>> GetById(int id, CancellationToken ct)
        {
            var customer = await _customerService.GetByIdAsync(id, ct);
            return Ok(customer);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateCustomerRequest updateCustomerRequest, CancellationToken ct)
        {
            if (id != updateCustomerRequest.Id)
                return BadRequest("Route Id and Request body Id do not match");

            var updated = await _customerService.UpdateAsync(id, updateCustomerRequest, ct);

            if (!updated)
                return BadRequest("Update failed.");

            return Ok("Updated");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _customerService.DeleteAsync(id, ct);

            if (!deleted)
                return BadRequest("Delete failed.");

            return Ok("Deleted");
        }
    }
}
