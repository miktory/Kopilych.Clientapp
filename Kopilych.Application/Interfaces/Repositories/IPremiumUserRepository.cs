using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IPremiumUserRepository
    {
        Task<PremiumUser> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<PremiumUser>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(PremiumUser premiumUser, CancellationToken ctoken);
        Task UpdateAsync(PremiumUser premiumUser);
        Task DeleteAsync(PremiumUser premiumUser);
		Task SaveChangesAsync(CancellationToken ctoken);
        Task<PremiumUser> GetForUserAsync(int id, CancellationToken ctoken);


    }
}
