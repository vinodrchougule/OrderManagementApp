using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.Users.Queries
{
    public class GetAppUserByIdQueryHandler : IRequestHandler<GetAppUserByIdQuery, AppUserResponse>
    {
        private readonly IAppUserRepository _appUserRepository;

        public GetAppUserByIdQueryHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<AppUserResponse> Handle(GetAppUserByIdQuery request, CancellationToken cancellationToken)
        {
            var appUser = await _appUserRepository.GetByIdAsync(request.Id, cancellationToken);

            if (appUser is null)
                throw new NotFoundException("AppUser", "id", request.Id);

            return AppUserMapper.ToResponse(appUser);
        }
    }
}
