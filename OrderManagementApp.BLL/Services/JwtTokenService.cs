using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderManagementApp.BLL.Interfaces;
using OrderManagementApp.Common.Settings;
using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagementApp.BLL.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateAccessToken(AppUser user)
        {
            var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry      = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);
                        
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                                                issuer: _jwtSettings.Issuer,
                                                audience: _jwtSettings.Audience,
                                                claims: claims,
                                                expires: expiry,
                                                signingCredentials: credentials
                                            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey    = true,
                ValidateIssuer              = true,
                ValidateAudience            = true,
                ValidateLifetime            = false,
                ValidIssuer                 = _jwtSettings.Issuer,
                ValidAudience               = _jwtSettings.Audience,
                IssuerSigningKey            = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!)),
            };

            var handler = new JwtSecurityTokenHandler();
            var claimPrincipal = handler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm.");
            }

            return claimPrincipal;
        }
    }
}
