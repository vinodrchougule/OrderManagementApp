using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApp.BLL.Features.Users.Commands;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.DTOs.Auth;
using System.Security.Claims;

namespace OrderManagementApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMediator _mediator;
        public AuthController(IAuthService authService, IMediator mediator)
        {
            _authService = authService;
            _mediator = mediator;
        }

        //Test changes to test on this laptop

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> RegisterUserAsync(RegisterUserRequest registerUserRequest, CancellationToken ct)
        {
            var authResponse = await _authService.RegisterAsync(registerUserRequest, ct);
            return Ok(authResponse);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> LoginAsync(LoginRequest loginRequest, CancellationToken ct)
        {
            var authResponse = await _authService.LoginAsync(loginRequest, ct);
            return Ok(authResponse);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> RefreshTokenAync([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken ct)
        {
            var response = await _authService.RefreshTokenAsync(refreshTokenRequest, ct);
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> RevokeToken(CancellationToken ct)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            
            if(userName is null)
                return Unauthorized();

            string result = await _authService.RevokeTokenAsync(userName, ct);
            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest changePasswordRequest, CancellationToken ct)
        {
            var command = new ChangePasswordCommand(
                changePasswordRequest.Username ?? string.Empty,
                changePasswordRequest.CurrentPassword ?? string.Empty,
                changePasswordRequest.NewPassword ?? string.Empty);

            var updated = await _mediator.Send(command, ct);

            if (!updated)
                return BadRequest("Password change failed.");

            return Ok("Password changed successfully.");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest, CancellationToken ct)
        {
            var command = new ForgotPasswordCommand(forgotPasswordRequest.Email ?? string.Empty);

            await _mediator.Send(command, ct);

            return Ok("If an account with that email exists, a password reset link has been sent.");
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest, CancellationToken ct)
        {
            var command = new ResetPasswordCommand(
                resetPasswordRequest.Token ?? string.Empty,
                resetPasswordRequest.NewPassword ?? string.Empty);

            var updated = await _mediator.Send(command, ct);

            if (!updated)
                return BadRequest("Password reset failed.");

            return Ok("Password has been reset successfully.");
        }
    }
}
