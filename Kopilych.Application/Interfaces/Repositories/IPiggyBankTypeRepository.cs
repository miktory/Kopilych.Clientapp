using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IPiggyBankTypeRepository
    {
        Task<PiggyBankType> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<PiggyBankType>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(PiggyBankType piggyBankType, CancellationToken ctoken);
        Task UpdateAsync(PiggyBankType piggyBankType);
        Task DeleteAsync(PiggyBankType piggyBankType);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
