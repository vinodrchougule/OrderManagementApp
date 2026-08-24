using FluentValidation;
using Microsoft.Extensions.Options;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.BLL.Validators;
using OrderManagementApp.Common.DTOs.Auth;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Common.Settings;
using OrderManagementApp.Domain.Entities;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly RegisterUserRequestValidator _registerUserRequestValidator;
        private readonly LoginRequestValidator _loginRequestValidator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly AccountLockoutSettings _accountLockoutSettings;

        public AuthService(IAppUserRepository appUserRepository, IOptions<JwtSettings> jwtSettings, IOptions<AccountLockoutSettings> accountLockoutSettings, IJwtTokenService jwtTokenService)
        {
            _appUserRepository = appUserRepository;
            _registerUserRequestValidator = new RegisterUserRequestValidator();
            _loginRequestValidator = new LoginRequestValidator();
            _jwtTokenService = jwtTokenService;
            _jwtSettings = jwtSettings.Value;
            _accountLockoutSettings = accountLockoutSettings.Value;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterUserRequest registerUserRequest, CancellationToken ct = default)
        {
            var ValidationResult = await _registerUserRequestValidator.ValidateAsync(registerUserRequest, ct);

            if (!ValidationResult.IsValid)
                throw new ValidationException(ValidationResult.Errors);

            var userByUsername = await _appUserRepository.GetByUsernameAsync(registerUserRequest.Username ?? "");

            if (userByUsername is not null)
                throw new ValidationException("Username already exist.");

            var userByEmail = await _appUserRepository.GetByEmailAsync(registerUserRequest.Email ?? "", ct);

            if (userByEmail is not null)
                throw new ValidationException("Email Address already exist.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerUserRequest.Password);

            var appUser = new AppUser
            {
                Username = registerUserRequest.Username,
                Email = registerUserRequest.Email,
                Role = registerUserRequest.Role,
                PasswordHash = passwordHash
            };

            var created = await _appUserRepository.CreateAsync(appUser, ct);

            var accessToken = _jwtTokenService.GenerateAccessToken(appUser);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            await _appUserRepository.UpdateRefreshTokenAsync(appUser.Id, refreshToken, refreshTokenExpiry, ct);

            return new AuthResponse
            {
                id = created.Id,
                Username = created.Username,
                Email = created.Email,
                Role = created.Role,
                Message = "Registration Successful.",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginRequest, CancellationToken ct)
        {
            var ValidationResult = await _loginRequestValidator.ValidateAsync(loginRequest, ct);

            if (!ValidationResult.IsValid)
                throw new ValidationException(ValidationResult.Errors);
            
            var userByUsername = await _appUserRepository.GetByUsernameAsync(loginRequest.UserName ?? "", ct);

            if (userByUsername is null)
                throw new ValidationException("Invalid Username");

            bool lockoutExpired = userByUsername.LockoutEndUtc.HasValue && userByUsername.LockoutEndUtc.Value <= DateTime.UtcNow;

            if (userByUsername.LockoutEndUtc.HasValue && !lockoutExpired)
            {
                var minutesRemaining = (int)Math.Ceiling((userByUsername.LockoutEndUtc.Value - DateTime.UtcNow).TotalMinutes);
                throw new ValidationException($"Account locked due to too many failed login attempts. Try again after {minutesRemaining} minute(s).");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, userByUsername.PasswordHash);

            if (!isPasswordValid)
            {
                int currentAttempts = lockoutExpired ? 0 : userByUsername.FailedLoginAttempts;
                int updatedAttempts = currentAttempts + 1;

                if (updatedAttempts >= _accountLockoutSettings.MaxFailedAttempts)
                {
                    var lockoutEndUtc = DateTime.UtcNow.AddMinutes(_accountLockoutSettings.LockoutDurationMinutes);
                    await _appUserRepository.UpdateLoginAttemptStatusAsync(userByUsername.Id, updatedAttempts, lockoutEndUtc, ct);
                    throw new ValidationException($"Account locked due to too many failed login attempts. Try again after {_accountLockoutSettings.LockoutDurationMinutes} minute(s).");
                }

                await _appUserRepository.UpdateLoginAttemptStatusAsync(userByUsername.Id, updatedAttempts, null, ct);
                int attemptsLeft = _accountLockoutSettings.MaxFailedAttempts - updatedAttempts;
                throw new ValidationException($"Incorrect password. You have {attemptsLeft} attempt(s) left.");
            }

            if (userByUsername.FailedLoginAttempts != 0 || userByUsername.LockoutEndUtc.HasValue)
                await _appUserRepository.UpdateLoginAttemptStatusAsync(userByUsername.Id, 0, null, ct);

            var accessToken = _jwtTokenService.GenerateAccessToken(userByUsername);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            await _appUserRepository.UpdateRefreshTokenAsync(userByUsername.Id, refreshToken, refreshTokenExpiry, ct);

            return new AuthResponse
            {
                id = userByUsername.Id,
                Username = userByUsername.Username,
                Email = userByUsername.Email,
                Role = userByUsername.Role,
                Message = "Login Successful.",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken ct)
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken ?? "");

            var userName = principal.Identity?.Name;
            if (userName is null)
                throw new UnauthorizedAccessException("Invalid access token.");

            var user = await _appUserRepository.GetByUsernameAsync(userName, ct);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid access name");

            if(user.RefreshToken != refreshTokenRequest.RefreshToken)
                throw new UnauthorizedAccessException("Invalid refresh token");

            if (user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token has expired. Please login again.");

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var expiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            await _appUserRepository.UpdateRefreshTokenAsync(user.Id,newRefreshToken, expiry,ct);

            return new AuthResponse
            {
                id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Message = "Token Refreshed Successfully.",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes)
            };
        }

        public async Task<string> RevokeTokenAsync(string Username, CancellationToken ct = default)
        {
            var user = await _appUserRepository.GetByUsernameAsync(Username, ct);

            if (user is null)
                throw new NotFoundException(nameof(AppUser),Username);

            await _appUserRepository.UpdateRefreshTokenAsync(user.Id, null, null, ct);
            return "Token revoked successfully.";
        }
    }
}
