using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
    public interface IUserInfoService
    {
        public Task<bool> CheckIfApprovedFriendRequestExistsAsync(int userId1, int userId2, CancellationToken cancellationToken);

        public Task<bool> CheckIfFriendRequestExistsAsync(int userId1, int userId2, CancellationToken cancellationToken);

        public Task<bool> CheckIfUserPremiumAsync(int userId, CancellationToken cancellationToken);

        public Task<UserDetailsVm> GetUserDetailsAsync(int userId, CancellationToken cancellationToken);

        public Task<UserDetailsVm> GetUserDetailsAsync(Guid externalUserGuid, CancellationToken cancellationToken);
        public Task<List<UserFriendshipDetailsVm>> GetAllUserFriendshipDetailsAsync(int userId, CancellationToken cancellationToken);
    }
}
