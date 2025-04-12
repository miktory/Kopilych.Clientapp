using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id, CancellationToken ctoken);
        Task<User> GetByGuidAsync(Guid guid, CancellationToken ctoken);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(User user, CancellationToken ctoken);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
