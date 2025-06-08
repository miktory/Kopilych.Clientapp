using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.CreateUser;
using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Application.CQRS.Commands.UserSession.CreateOrUpdateUserSession;
using Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus;
using Kopilych.Application.CQRS.Queries.User.GetCurrentUserDetails;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetAllUserFriendshipDetails;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.CQRS.Queries.UserSession.GetSessionByUserId;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kopilych.Application.Services
{
    internal class UserInfoService : IUserInfoService
    {
        IMediator _mediator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IApiCommunicatorService _apiCommunicatorService;
        private readonly IIntegrationService _integrationService;
        private IFileService _fileService;
        private IMapper _mapper;

        public UserInfoService(IServiceScopeFactory serviceScopeFactory, IApiCommunicatorService apiCommunicatorService, IIntegrationService integrationService, IMapper mapper, IMediator mediator, IFileService fileService)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _apiCommunicatorService = apiCommunicatorService;
            _integrationService = integrationService;
            _mapper = mapper;

            _fileService = fileService;
        }

        public async Task<bool> CheckIfApprovedFriendRequestExistsAsync(int userId1, int userId2, CancellationToken cancellationToken)
        {
            try
            {
                var friendship = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetFriendshipDetailsByUserIdsQuery { FirstUserId = userId1, SecondUserId = userId2, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                if (!friendship.RequestApproved)
                    return false;
            }
            catch (NotFoundException ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CheckIfFriendRequestExistsAsync(int userId1, int userId2, CancellationToken cancellationToken)
        {
            try
            {
                var friendship = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetFriendshipDetailsByUserIdsQuery { FirstUserId = userId1, SecondUserId = userId2, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            }
            catch (NotFoundException ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> CheckIfUserPremiumAsync(int userId, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPremiumStatusQuery { UserId = userId }, cancellationToken))).Premium;
        }
        public async Task<UserDetailsDTO> GetUserDetailsAsync(int userId, CancellationToken cancellationToken, bool remote)
        {
            UserDetailsDTO userDetailsDTO = null;
            if (!remote)
                userDetailsDTO = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserDetailsByIdQuery { Id = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            else
            {
                userDetailsDTO = await _integrationService.GetUserFromServerAsync(userId, cancellationToken);
                userDetailsDTO.ExternalId = userDetailsDTO.Id;
                userDetailsDTO.PhotoPath = _integrationService.GetUserPhotoURL(userId);
            }
            return userDetailsDTO;
        }
        public async Task<byte[]> GetUserPhotoAsync(UserDetailsDTO user, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                return await _integrationService.GetUserPhotoFromServerAsync(user.ExternalId.Value, cancellationToken);
            else
                return await _fileService.ReadFileAsync(user.PhotoPath);
        }


        public async Task<UserFriendshipDetailsDTO> GetFriendRequestAsync(int id, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                return await _integrationService.GetUserFriendshipFromServerAsync(id, cancellationToken);
            else
                throw new NotImplementedException();
        }

        public async Task<int> CreateFriendRequestAsync(CreateFriendshipDTO createFriendshipDTO, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                return await _integrationService.CreateUserFriendshipOnServerAsync(createFriendshipDTO, cancellationToken);
            else
                throw new NotImplementedException();
        }

        public async Task DeleteFriendRequestAsync(int id, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                await _integrationService.DeleteUserFriendshipFromServerAsync(id, cancellationToken);
            else
                throw new NotImplementedException();
        }
        public async Task UpdateFriendRequestAsync(int id, UpdateFriendshipDTO updateFriendshipDTO, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                await _integrationService.UpdateUserFriendshipOnServerAsync(id, updateFriendshipDTO, cancellationToken);
            else
                throw new NotImplementedException();
        }

        public async Task<UserDetailsDTO> GetUserDetailsByExternalIdAsync(int externalId, CancellationToken cancellationToken)
        {
            return await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserDetailsByExternalIdQuery { ExternalId = externalId }, cancellationToken));
        }

        public async Task<List<UserFriendshipDetailsDTO>> GetAllUserFriendshipDetailsAsync(int userId, CancellationToken cancellationToken, bool remote)
        {
            if (remote)
                return await _integrationService.GetUserFriendshipsByUserIdFromServerAsync(userId, cancellationToken);
            else
            {
                var list = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetAllUserFriendshipDetailsQuery { InitiatorUserId = 0, IsExecuteByAdmin = true, UserId = userId }, cancellationToken));
                return list;
            }
        }

        private async Task<T> ExecuteWithMediator<T>(Func<IMediator, Task<T>> func)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await func(_mediator);
            }
        }

        public async Task<UserDetailsDTO> GetCurrentUserDetailsAsync(CancellationToken cancellationToken, bool remote)
        {
            var currentUser = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCurrentUserDetailsQuery { }, cancellationToken));
            if (remote)
            {
                currentUser = await _integrationService.GetCurrentUserFromServerAsync(cancellationToken);
                currentUser.ExternalId = currentUser.Id;
            }
            return currentUser; 

        }

        public async Task<int> CreateUserAsync(string username, string photoPath, int? externalId, CancellationToken cancellationToken)
        {
            var result = await ExecuteWithMediator(async mediator => await _mediator.Send(new CreateUserCommand { ExternalId = externalId,PhotoPath = photoPath, InitiatorUserId = 0, IsExecuteByAdmin = true, Username = username}, cancellationToken));
            return result;
        }

        public async Task<Unit> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
                await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateUserCommand { Id = id, ExternalId = updateUserDTO.ExternalId, Version = updateUserDTO.Version.Value, InitiatorUserId = 0, IsExecuteByAdmin = true, Username = updateUserDTO.Username, PhotoPath = updateUserDTO.PhotoPath }, cancellationToken));
            else
                await _integrationService.UpdateUserOnServerAsync(id, updateUserDTO, cancellationToken);

            return Unit.Value;
        }

        public async Task<int> СreateOrUpdateUserSessionAsync(CreateOrUpdateUserSessionDTO createOrUpdateUserSessionDTO, CancellationToken cancellationToken)
        {
            var result = await ExecuteWithMediator(async mediator => await _mediator.Send( new CreateOrUpdateUserSessionCommand { UserId = createOrUpdateUserSessionDTO.UserId, RefreshToken = createOrUpdateUserSessionDTO.RefreshToken, AccessToken = createOrUpdateUserSessionDTO.AccessToken  }, cancellationToken));
            return result;
        }

        public async Task LogoutAsync(bool full, LogoutDTO logoutDTO, CancellationToken cancellationToken)
        {
            await _integrationService.LogoutAsync(full, logoutDTO, cancellationToken);
        }

        public async Task<UserSessionDTO> GetCurrentUserSessionAsync(CancellationToken cancellationToken)
        {
            var user = await GetCurrentUserDetailsAsync(cancellationToken, false);
            var result = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetSessionByUserIdQuery { UserId = user.Id }, cancellationToken));
            return result;
        }

        public async Task<UserDetailsDTO> RunTwoWayUserIntegration(UserDetailsDTO localUser, UserDetailsDTO externalUser, CancellationToken cancellationToken)
        {
            return await _integrationService.RunTwoWayUserIntegration(localUser, externalUser, cancellationToken);
        }

        public async Task<string> UploadUserPhotoAsync(int userId, byte[] photo, CancellationToken cancellationToken, bool remote)
        {
            var u = await GetUserDetailsAsync(userId, cancellationToken, remote);
            if (!remote)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    _fileService = scope.ServiceProvider.GetRequiredService<IFileService>();
                    string jpegPath = Path.Combine(_fileService.GetDefaultPath(), $"user_{u.Id}_{Guid.NewGuid().ToString()}.jpeg");

                    if (_fileService.Exist(jpegPath))
                        await _fileService.RemoveFileAsync(jpegPath);

                    await _fileService.CreateFileAsync(jpegPath, photo, true);
                    var updateDto = _mapper.Map<UpdateUserDTO>(u);
                    updateDto.PhotoPath = jpegPath;
                    await UpdateUserAsync(userId, updateDto, cancellationToken, false);

                    if (_fileService.Exist(u.PhotoPath))
                        await _fileService.RemoveFileAsync(u.PhotoPath);
                    return jpegPath;
                }
            }
            else
            {
                await _integrationService.UploadPiggyBankCustomizationPhotoOnServerAsync(userId, photo, cancellationToken);
                return _integrationService.GetUserPhotoURL(userId);
            }
        }
    }
}
