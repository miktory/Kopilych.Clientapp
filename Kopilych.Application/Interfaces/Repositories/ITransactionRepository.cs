using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken ctoken);
        Task<IEnumerable<Transaction>> GetAllForUserAsync(int userId, CancellationToken ctoken);
        Task<IEnumerable<Transaction>> GetAllForPiggyBankAsync(int piggyBankId, CancellationToken ctoken);
        Task AddAsync(Transaction transaction, CancellationToken ctoken);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
		Task SaveChangesAsync(CancellationToken ctoken);

    }
}
