using FluentValidation;
using MediatR;
using OrderManagementApp.BLL.Mappers;
using OrderManagementApp.Common.Exceptions;
using OrderManagementApp.Domain.Interfaces;

namespace OrderManagementApp.BLL.Features.Users.Commands
{
    public class UpdateAppUserCommandHandler : IRequestHandler<UpdateAppUserCommand, bool>
    {
        private readonly IAppUserRepository _appUserRepository;

        public UpdateAppUserCommandHandler(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public async Task<bool> Handle(UpdateAppUserCommand request, CancellationToken cancellationToken)
        {
            var usernameExists = await _appUserRepository.ExistsByUsernameAsync(request.Username, request.Id, cancellationToken);

            if (usernameExists)
                throw new ValidationException("Username already exists.");

            var emailExists = await _appUserRepository.ExistsByEmailAsync(request.Email, request.Id, cancellationToken);

            if (emailExists)
                throw new ValidationException("Email already exists.");

            var appUser = AppUserMapper.ToEntity(request);

            var updated = await _appUserRepository.UpdateAsync(request.Id, appUser, cancellationToken);

            if (!updated)
                throw new NotFoundException("AppUser", "id", request.Id);

            return updated;
        }
    }
}
