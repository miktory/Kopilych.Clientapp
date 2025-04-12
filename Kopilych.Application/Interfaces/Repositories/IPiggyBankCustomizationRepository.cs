using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IPiggyBankCustomizationRepository
    {
        Task<PiggyBankCustomization> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<PiggyBankCustomization>> GetAllAsync(CancellationToken ctoken);
        Task<IEnumerable<PiggyBankCustomization>> GetAllForPiggyBankAsync(int piggyBankId, CancellationToken ctoken);
        Task<IEnumerable<PiggyBankCustomization>> GetAllForPiggyBankTypeAsync(int piggyBankTypeId, CancellationToken ctoken);
        Task AddAsync(PiggyBankCustomization piggyBankCustomization, CancellationToken ctoken);
        Task UpdateAsync(PiggyBankCustomization piggyBankCustomization);
        Task DeleteAsync(PiggyBankCustomization piggyBankCustomization);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
