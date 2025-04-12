using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface ITransactionTypeRepository
    {
        Task<TransactionType> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<TransactionType>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(TransactionType transactionType, CancellationToken ctoken);
        Task UpdateAsync(TransactionType transactionType);
        Task DeleteAsync(TransactionType transactionType);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
