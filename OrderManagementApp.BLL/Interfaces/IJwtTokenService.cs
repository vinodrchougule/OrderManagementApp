using OrderManagementApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace OrderManagementApp.BLL.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(AppUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken);
    }
}
