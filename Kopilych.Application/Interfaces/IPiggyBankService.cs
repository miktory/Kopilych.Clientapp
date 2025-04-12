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
        public Task<PiggyBankVm> GetPiggyBankDetailsAsync(int piggyBankId, CancellationToken cancellationToken);
        public Task<int> GetUserPiggyBankLinksCountForPiggyBank(int piggyBankId, CancellationToken cancellationToken);
        public Task<int> GetCommonPiggyBanksCountForUsersAsync(int userId1, int userId2, CancellationToken cancellationToken);
        public Task<int> GetCurrentPiggyBanksCountForUserAsync(int userId, CancellationToken cancellationToken);
        public Task<UserPiggyBankVm> GetUserPiggyBankLinkByUserIdAndPiggyBankId(int userId, int piggyBankId, CancellationToken cancellationToken);
        public Task UnlinkAllUsersExceptOwnerAsync(int piggyBankId, CancellationToken ctoken);
        public Task<PiggyBankTypeDTO> GetPiggyBankTypeDetailsAsync(int id, CancellationToken cancellationToken);
    }
        
}
