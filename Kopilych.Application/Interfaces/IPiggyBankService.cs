using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
    public interface IPiggyBankService
    {
        public Task<PiggyBankDTO> GetPiggyBankDetailsAsync(int piggyBankId, CancellationToken cancellationToken, bool remote);
        public Task<PiggyBankDTO> GetPiggyBankDetailsByExternalId(int piggyBankId, CancellationToken cancellationToken);
        public Task<int> GetUserPiggyBankLinksCountForPiggyBank(int piggyBankId, CancellationToken cancellationToken);
        public Task<int> GetCommonPiggyBanksCountForUsersAsync(int userId1, int userId2, CancellationToken cancellationToken);
        public Task<int> GetCurrentPiggyBanksCountForUserAsync(int userId, CancellationToken cancellationToken);
        public Task<UserPiggyBankDTO> GetUserPiggyBankLinkByUserIdAndPiggyBankId(int userId, int piggyBankId, CancellationToken cancellationToken);
        public Task UnlinkAllUsersExceptOwnerAsync(int piggyBankId, CancellationToken ctoken);
        public Task<PiggyBankTypeDTO> GetPiggyBankTypeDetailsAsync(int id, CancellationToken cancellationToken);
        public Task<List<int>> CreatePiggyBankWithCustomizationAndLinkToUser(CreatePiggyBankDTO piggyBank, CreatePiggyBankCustomizationDTO piggyBankCustomization, CancellationToken cancellationToken, bool remote);
        public Task<List<PiggyBankTypeDTO>> GetAllPiggyBankTypesAsync(CancellationToken cancellationToken);
        public Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationByPiggyBankIdAsync(int piggyBankId, CancellationToken cancellationToken, bool remote);
        public Task<byte[]> GetPiggyBankCustomizationPhotoAsync(PiggyBankCustomizationDTO piggyBankCustomization, CancellationToken cancellationToken, bool remote);
        public Task<string> UploadPiggyBankCustomizationPhotoAsync(int piggyBankCustomizationId, byte[] photo, CancellationToken cancellationToken, bool remote);
        public Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByUserIdAsync(int userId, CancellationToken cancellationToken, bool remote);
        public Task<List<UserPiggyBankDTO>> GetUserPiggyBankLinksByPiggyBankIdAsync(int piggyBankId, CancellationToken cancellationToken, bool remote);
        public Task DeleteUserPiggyBankAsync(int userPiggyBankId, CancellationToken cancellationToken, bool remote);
        public Task UpdatePiggyBankAsync(int piggyBankId, UpdatePiggyBankDTO updatePiggyBankDTO, CancellationToken cancellationToken, bool remote);
        public Task UpdatePiggyBankCustomizationAsync(int piggyBankCustomizationId, UpdatePiggyBankCustomizationDTO updatePiggyBankCustomizationDTO, CancellationToken cancellationToken, bool remote);
        public Task<PiggyBankCustomizationDTO> GetPiggyBankCustomizationByIdAsync(int piggyBankCustomizationId, CancellationToken cancellationToken, bool remote);
        public Task<int> CreatePiggyBankAsync(CreatePiggyBankDTO piggyBank, CancellationToken cancellationToken, bool remote);
        public Task DeletePiggyBankAsync(int piggyBankId, CancellationToken cancellationToken, bool remote);

        public Task<int> CreatePiggyBankCustomizationAsync(CreatePiggyBankCustomizationDTO piggyBankCustomization, CancellationToken cancellationToken, bool remote);

        public Task UpdateUserPiggyBankLinkAsync(int userPiggyBankId, UpdateUserPiggyBankDTO updateUserPiggyBank, CancellationToken cancellationToken, bool remote);

        public Task<int> CreateUserPiggyBankLinkAsync(CreateUserPiggyBankDTO userPiggyBank, CancellationToken cancellationToken, bool remote);

        public Task<PiggyBankDTO> RunTwoWayPiggyBankIntegration(PiggyBankDTO? localPiggyBank, PiggyBankDTO? externalPiggyBank, CancellationToken cancellationToken);
        public Task<UserPiggyBankDTO> RunTwoWayUserPiggyBankIntegration(UserPiggyBankDTO? localUserPiggyBank, UserPiggyBankDTO? externalUserPiggyBank, CancellationToken cancellationToken);
        public Task<UserPiggyBankDTO> GetUserPiggyBankLinkAsync(int userPiggyBankId, CancellationToken cancellationToken, bool remote);
        public Task<PiggyBankCustomizationDTO> RunTwoWayPiggyBankCustomizationIntegration(PiggyBankCustomizationDTO? localPiggyBankCustomization, PiggyBankCustomizationDTO? externalPiggyBankCustomization, CancellationToken cancellationToken);
    }
        
}
