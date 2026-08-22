using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.DTOs;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.Users.Queries
{
    public class GetAllAppUsersQueryHandler : IRequestHandler<GetAllAppUsersQuery, List<AppUserResponse>>
    {
        private readonly IAppUserRepository _appUserRepository;

        public GetAllAppUsersQueryHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<List<AppUserResponse>> Handle(GetAllAppUsersQuery request, CancellationToken cancellationToken)
        {
            var appUsers = await _appUserRepository.GetAllAsync(cancellationToken);

            return AppUserMapper.ToResponseList(appUsers);
        }
    }
}
