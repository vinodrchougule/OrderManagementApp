using OrderManagementApp.Common.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterUserRequest registerUserRequest, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(LoginRequest loginRequest, CancellationToken ct);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken ct);
        Task<string> RevokeTokenAsync(string Username, CancellationToken ct = default);
    }
}
