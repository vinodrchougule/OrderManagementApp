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
    }
}
