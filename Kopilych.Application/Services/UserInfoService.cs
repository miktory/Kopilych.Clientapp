using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetAllUserFriendshipDetails;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
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

        public UserInfoService(IServiceScopeFactory serviceScopeFactory, IMediator mediator)
        {
            _serviceScopeFactory = serviceScopeFactory;
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

        public async Task<UserDetailsVm> GetUserDetailsAsync(int userId, CancellationToken cancellationToken)
        {
           return await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserDetailsByIdQuery { Id = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
        }

        public async Task<UserDetailsVm> GetUserDetailsAsync(Guid externalUserGuid, CancellationToken cancellationToken)
        {
            return await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserDetailsByExternalIdQuery { ExternalId = externalUserGuid }, cancellationToken));
        }

        public async Task<List<UserFriendshipDetailsVm>> GetAllUserFriendshipDetailsAsync(int userId, CancellationToken cancellationToken)
        {
            return await ExecuteWithMediator(async mediator => await _mediator.Send(new GetAllUserFriendshipDetailsQuery { InitiatorUserId = 0, IsExecuteByAdmin = true, UserId = userId }, cancellationToken));
        }

        private async Task<T> ExecuteWithMediator<T>(Func<IMediator, Task<T>> func)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await func(_mediator);
            }
        }
    }
}
