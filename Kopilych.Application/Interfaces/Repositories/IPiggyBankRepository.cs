using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IPiggyBankRepository
    {
        Task<PiggyBank> GetByIdAsync(int id, CancellationToken ctoken);
        Task<PiggyBank> GetByExternalIdAsync(int externalId, CancellationToken ctoken);
        Task<IEnumerable<PiggyBank>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(PiggyBank userPiggyBank, CancellationToken ctoken);
        Task UpdateAsync(PiggyBank userPiggyBank);
        Task DeleteAsync(PiggyBank userPiggyBank);
		Task SaveChangesAsync(CancellationToken ctoken);
        Task<IEnumerable<PiggyBank>> GetAllForUserAsync(int id, CancellationToken ctoken);

    }
}
