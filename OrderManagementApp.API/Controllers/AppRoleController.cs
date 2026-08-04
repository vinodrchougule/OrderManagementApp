using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppRoleController : ControllerBase
    {
        private readonly IAppRoleService _appRoleService;
        private readonly ILogger<AppRoleController> _logger;

        public AppRoleController(IAppRoleService appRoleService, ILogger<AppRoleController> logger)
        {
            _appRoleService = appRoleService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<AppRoleResponse>> Create([FromBody] CreateAppRoleRequest createAppRoleRequest, CancellationToken ct)
        {
            var appRole = await _appRoleService.CreateAsync(createAppRoleRequest, ct);
            return Ok(appRole);
        }

        [HttpGet]
        public async Task<ActionResult<List<AppRoleResponse>>> GetAll(CancellationToken ct)
        {
            var appRoles = await _appRoleService.GetAllAsync(ct);
            return Ok(appRoles);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<AppRoleResponse>> GetById(long id, CancellationToken ct)
        {
            var appRole = await _appRoleService.GetByIdAsync(id, ct);
            return Ok(appRole);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateAppRoleRequest updateAppRoleRequest, CancellationToken ct)
        {
            if (id != updateAppRoleRequest.Id)
                return BadRequest("Route Id and Request body Id do not match");

            var updated = await _appRoleService.UpdateAsync(id, updateAppRoleRequest, ct);

            if (!updated)
                return BadRequest("Update failed.");

            return Ok("Updated");
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> Delete(long id, CancellationToken ct)
        {
            var deleted = await _appRoleService.DeleteAsync(id, ct);

            if (!deleted)
                return BadRequest("Delete failed.");

            return Ok("Deleted");
        }
    }
}
