using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApp.BLL.Features.AppRoles.Commands;
using OrderManagementApp.BLL.Features.AppRoles.Queries;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppRoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppRoleController> _logger;

        public AppRoleController(IMediator mediator, ILogger<AppRoleController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AppRoleResponse>> Create([FromBody] CreateAppRoleRequest createAppRoleRequest, CancellationToken ct)
        {
            var appRole = await _mediator.Send(new CreateAppRoleCommand(createAppRoleRequest.RoleName), ct);
            return Ok(appRole);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<AppRoleResponse>>> GetAll(CancellationToken ct)
        {
            var appRoles = await _mediator.Send(new GetAllAppRolesQuery(), ct);
            return Ok(appRoles);
        }

        [HttpGet("{id:long}")]
        [Authorize]
        public async Task<ActionResult<AppRoleResponse>> GetById(long id, CancellationToken ct)
        {
            var appRole = await _mediator.Send(new GetAppRoleByIdQuery(id), ct);
            return Ok(appRole);
        }

        [HttpPut("{id:long}")]
        [Authorize]
        public async Task<ActionResult> Update(long id, [FromBody] UpdateAppRoleRequest updateAppRoleRequest, CancellationToken ct)
        {
            if (id != updateAppRoleRequest.Id)
                return BadRequest("Route Id and Request body Id do not match");

            var updated = await _mediator.Send(new UpdateAppRoleCommand(id, updateAppRoleRequest.RoleName), ct);

            if (!updated)
                return BadRequest("Update failed.");

            return Ok("Updated");
        }

        [HttpDelete("{id:long}")]
        [Authorize]
        public async Task<ActionResult> Delete(long id, CancellationToken ct)
        {
            var deleted = await _mediator.Send(new DeleteAppRoleCommand(id), ct);

            if (!deleted)
                return BadRequest("Delete failed.");

            return Ok("Deleted");
        }
    }
}
