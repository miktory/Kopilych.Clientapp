using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
    public interface IIntegrationService
    {
        // это server-side
        public string LoginPageAddress { get; }

        public bool IsConfigured { get;  }
        public bool IsSessionExists { get; }

        public event Action AuthCompleted;

        public Task Configure();
        public Task<UserDetailsDTO> GetUserFromServerAsync(int userId, CancellationToken cancellationToken);
        public Task<UserDetailsDTO> GetCurrentUserFromServerAsync(CancellationToken cancellationToken);
        public Task UpdateUserOnServerAsync(int userId, UpdateUserDTO updateUserDTO, CancellationToken cancellationToken);
        public Task<int> CreateUserOnServerAsync(CreateUserDTO createUserDTO, CancellationToken cancellationToken);
        public Task DeleteUserFromServerAsync(int userId, CancellationToken cancellationToken);
        public Task<byte[]> GetUserPhotoFromServerAsync(int userId, CancellationToken cancellationToken);
        public Task DeleteUserPhotoFromServerAsync(int userId, CancellationToken cancellationToken);
        public Task UploadUserPhotoOnServerAsync(int userId, byte[] data, CancellationToken cancellationToken);

        public Task<UserFriendshipDetailsDTO> GetUserFriendshipFromServerAsync(int userFriendshipId, CancellationToken cancellationToken);
        public Task<List<UserFriendshipDetailsDTO>> GetUserFriendshipsByUserIdFromServerAsync(int userId, CancellationToken cancellationToken);
        public Task <int> CreateUserFriendshipOnServerAsync(CreateFriendshipDTO createFriendshipDTO, CancellationToken cancellationToken);
        public Task UpdateUserFriendshipOnServerAsync(int userFriendshipId, UpdateFriendshipDTO updateFriendshipDTO, CancellationToken cancellationToken);
        public Task DeleteUserFriendshipFromServerAsync(int userFriendshipId, CancellationToken cancellationToken);


        public Task<PiggyBankDTO> GetPiggyBankFromServerAsync(int piggyBankId, CancellationToken cancellationToken);
        public Task UpdatePiggyBankOnServerAsync(int piggyBankId, UpdatePiggyBankDTO updatePiggyBankDTO, CancellationToken cancellationToken);
        public Task<int> CreatePiggyBankOnServerAsync(CreatePiggyBankDTO createPiggyBankDTO, CancellationToken cancellationToken);
        public Task DeletePiggyBankFromServerAsync(int piggyBankId, CancellationToken cancellationToken);

        public Task<UserPiggyBankDTO> GetUserPiggyBankLinkFromServerAsync(int userPiggyBankLinkId, CancellationToken cancellationToken);
        public Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByUserIdFromServerAsync(int userId, CancellationToken cancellationToken);
        public Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByPiggyBankIdFromServerAsync(int piggyBankId, CancellationToken cancellationToken);
        public Task UpdateUserPiggyBankLinkOnServerAsync(int userPiggyBankId, UpdateUserPiggyBankDTO updateUserPiggyBankDTO, CancellationToken cancellationToken);
        public Task<int> CreateUserPiggyBankLinkOnServerAsync(CreateUserPiggyBankDTO createUserPiggyBankDTO, CancellationToken cancellationToken);
        public Task DeleteUserPiggyBankLinkFromServerAsync(int userPiggyBankId, CancellationToken cancellationToken);

        public Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken);
        public Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationByPiggyBankIdFromServerAsync(int piggyBankId, CancellationToken cancellationToken);
        public Task UpdatePiggyBankCustomizationOnServerAsync(int piggyBankCustomizationId, UpdatePiggyBankCustomizationDTO updatePiggyBankCustomizationDTO, CancellationToken cancellationToken);
        public Task<int> CreatePiggyBankCustomizationOnServerAsync(CreatePiggyBankCustomizationDTO createPiggyBankCustomizationDTO, CancellationToken cancellationToken);
        public Task DeleteUserPiggyBankCustomizationFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken);
        public Task<byte[]> GetPiggyBankCustomizationPhotoFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken);
        public Task DeletePiggyBankCustomizationPhotoFromServerAsync(int piggyBankCustomizationId, CancellationToken cancellationToken);
        public Task UploadPiggyBankCustomizationPhotoOnServerAsync(int piggyBankCustomizationId, byte[] data, CancellationToken cancellationToken);

        public Task<TransactionDTO> GetTransactionFromServerAsync(int transactionId, CancellationToken cancellationToken);
        public Task<List<TransactionDTO>> GetTransactionsByPiggyBankIdFromServerAsync(int piggyBankId, CancellationToken cancellationToken);
        public Task UpdateTransactionOnServerAsync(int transactionId, UpdateTransactionDTO updateTransactionDTO, CancellationToken cancellationToken, bool updatePiggyBankBalance = true);
        public Task<int> CreateTransactionOnServerAsync(CreateTransactionDTO createTransactionDTO, CancellationToken cancellationToken, bool updatePiggyBankBalance = true);
        public Task DeleteTransactionFromServerAsync(int transactionId, CancellationToken cancellationToken, bool updatePiggyBankBalance = true);

        public Task<PiggyBankTypeDTO> GetPiggyBankTypeFromServerAsync(int piggyBankTypeId, CancellationToken cancellationToken);

        public Task<TransactionTypeDTO> GetTransactionTypeFromServerAsync(int transactionTypeId, CancellationToken cancellationToken);

        public Task<PaymentTypeDTO> GetPaymentTypeFromServerAsync(int paymentTypeId, CancellationToken cancellationToken);

        public Task<bool> CheckIfServerOnline(CancellationToken cancellationToken);
        public Task<bool> CheckIfAccessTokenValid(CancellationToken cancellationToken);

        public Task<UserDetailsDTO> RunTwoWayUserIntegration(UserDetailsDTO localUser, UserDetailsDTO externalUser, CancellationToken cancellationToken);
        public Task<PiggyBankDTO> RunTwoWayPiggyBankIntegration(PiggyBankDTO? localPiggyBank, PiggyBankDTO? externalPiggyBank, CancellationToken cancellationToken);
        public Task<UserPiggyBankDTO> RunTwoWayUserPiggyBankIntegration(UserPiggyBankDTO? localUserPiggyBank, UserPiggyBankDTO? externalUserPiggyBank, CancellationToken cancellationToken);
        public Task<PiggyBankCustomizationDTO> RunTwoWayPiggyBankCustomizationIntegration(PiggyBankCustomizationDTO? localPiggyBankCustomization, PiggyBankCustomizationDTO? externalPiggyBankCustomization, CancellationToken cancellationToken);

        public Task LogoutAsync(bool full, LogoutDTO logoutDTO, CancellationToken cancellationToken);

        public void FinishAuth();

        public Task<TransactionDTO> RunTwoWayTransactionIntegration(TransactionDTO? localTransaction, TransactionDTO? externalTransaction, CancellationToken cancellationToken);


        public string GetPiggyBankCustomizationPhotoURL(int piggyBankCustomizationId);
        public string GetUserPhotoURL(int userId);
        public string? GetCurrentAccessToken();

    }
}
