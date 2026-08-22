using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApp.BLL.Features.Users.Commands;
using OrderManagementApp.BLL.Features.Users.Queries;
using OrderManagementApp.Common.DTOs;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppUserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AppUserController> _logger;

        public AppUserController(IMediator mediator, ILogger<AppUserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<AppUserResponse>>> GetAll(CancellationToken ct)
        {
            var appUsers = await _mediator.Send(new GetAllAppUsersQuery(), ct);
            return Ok(appUsers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AppUserResponse>> GetById(int id, CancellationToken ct)
        {
            var appUser = await _mediator.Send(new GetAppUserByIdQuery(id), ct);
            return Ok(appUser);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateAppUserRequest updateAppUserRequest, CancellationToken ct)
        {
            if (id != updateAppUserRequest.Id)
                return BadRequest("Route Id and Request body Id do not match");

            var updated = await _mediator.Send(new UpdateAppUserCommand(id, updateAppUserRequest.Username, updateAppUserRequest.Email, updateAppUserRequest.Role), ct);

            if (!updated)
                return BadRequest("Update failed.");

            return Ok("Updated");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _mediator.Send(new DeleteAppUserCommand(id), ct);

            if (!deleted)
                return BadRequest("Delete failed.");

            return Ok("Deleted");
        }
    }
}
