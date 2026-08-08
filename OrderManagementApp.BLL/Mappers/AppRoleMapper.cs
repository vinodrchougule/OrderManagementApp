using OrderManagementApp.BLL.Features.AppRoles.Commands;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderManagementApp.BLL.Mappers
{
    [Mapper]
    public static partial class AppRoleMapper
    {
        [MapperIgnoreTarget(nameof(AppRole.AppRoleId))]
        public static partial AppRole ToEntity(CreateAppRoleCommand command);

        [MapProperty(nameof(UpdateAppRoleCommand.Id), nameof(AppRole.AppRoleId))]
        public static partial AppRole ToEntity(UpdateAppRoleCommand command);

        [MapProperty(nameof(AppRole.AppRoleId), nameof(AppRoleResponse.Id))]
        public static partial AppRoleResponse ToResponse(AppRole appRole);

        public static partial List<AppRoleResponse> ToResponseList(List<AppRole> appRoles);
    }
}
