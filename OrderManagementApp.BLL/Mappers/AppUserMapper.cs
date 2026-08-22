using OrderManagementApp.BLL.Features.Users.Commands;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Mappers
{
    [Mapper]
    public static partial class AppUserMapper
    {
        [MapperIgnoreSource(nameof(AppUser.PasswordHash))]
        [MapperIgnoreSource(nameof(AppUser.RefreshToken))]
        [MapperIgnoreSource(nameof(AppUser.RefreshTokenExpiry))]
        public static partial AppUserResponse ToResponse(AppUser appUser);

        public static partial List<AppUserResponse> ToResponseList(List<AppUser> appUsers);

        [MapperIgnoreTarget(nameof(AppUser.PasswordHash))]
        [MapperIgnoreTarget(nameof(AppUser.RefreshToken))]
        [MapperIgnoreTarget(nameof(AppUser.RefreshTokenExpiry))]
        public static partial AppUser ToEntity(UpdateAppUserCommand command);
    }
}
