using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.Services;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
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

        public Task<UserDetailsDTO> GetUserDetailsAsync(int userId, CancellationToken cancellationToken, bool remote);

        public Task<byte[]> GetUserPhotoAsync(UserDetailsDTO user, CancellationToken cancellationToken, bool remote);

        public Task<string> UploadUserPhotoAsync(int userId, byte[] photo, CancellationToken cancellationToken, bool remote);

        public Task<UserDetailsDTO> GetCurrentUserDetailsAsync(CancellationToken cancellationToken, bool remote);

        public Task<int> CreateUserAsync(string username, string photoPath, int? externalId, CancellationToken cancellationToken);

        public Task LogoutAsync(bool full, LogoutDTO logoutDTO, CancellationToken cancellationToken);

        public Task<UserSessionDTO> GetCurrentUserSessionAsync(CancellationToken cancellationToken);

        public Task<UserFriendshipDetailsDTO> GetFriendRequestAsync(int id, CancellationToken cancellationToken, bool remote);
        public Task<int> CreateFriendRequestAsync(CreateFriendshipDTO createFriendshipDTO, CancellationToken cancellationToken, bool remote);
        public Task DeleteFriendRequestAsync(int id, CancellationToken cancellationToken, bool remote);
        public Task UpdateFriendRequestAsync(int id, UpdateFriendshipDTO updateFriendshipDTO, CancellationToken cancellationToken, bool remote);
   

        public Task<Unit> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken, bool remote);

        public Task<UserDetailsDTO> GetUserDetailsByExternalIdAsync(int externalId, CancellationToken cancellationToken);
        public Task<List<UserFriendshipDetailsDTO>> GetAllUserFriendshipDetailsAsync(int userId, CancellationToken cancellationToken, bool remote);
        public Task<int> СreateOrUpdateUserSessionAsync(CreateOrUpdateUserSessionDTO createOrUpdateUserSessionDTO,  CancellationToken cancellationToken);

        public Task<UserDetailsDTO> RunTwoWayUserIntegration(UserDetailsDTO localUser, UserDetailsDTO externalUser, CancellationToken cancellationToken);
    }
}
