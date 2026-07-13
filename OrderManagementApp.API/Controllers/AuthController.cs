using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

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
    }
}
