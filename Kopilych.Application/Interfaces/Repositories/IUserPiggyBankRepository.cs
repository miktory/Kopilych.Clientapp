using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IUserPiggyBankRepository
    {
        Task<UserPiggyBank> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<UserPiggyBank>> GetAllAsync(CancellationToken ctoken);
        Task<IEnumerable<UserPiggyBank>> GetAllForUserAsync(int userId, CancellationToken ctoken);
        Task<IEnumerable<UserPiggyBank>> GetAllForPiggyBankAsync(int piggyBankId, CancellationToken ctoken);
        Task<IEnumerable<int>> GetCommonPiggyBankIdsForUsersAsync (int firstUserId, int secondUserId, CancellationToken ctoken);
        Task<UserPiggyBank> GetByUserIdAndPiggyBankIdAsync(int userId, int piggyBankId, CancellationToken ctoken);

        Task AddAsync(UserPiggyBank userPiggyBank, CancellationToken ctoken);
        Task UpdateAsync(UserPiggyBank userPiggyBank);
        Task DeleteAsync(UserPiggyBank userPiggyBank);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
