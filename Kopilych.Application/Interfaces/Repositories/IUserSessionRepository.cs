using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IUserSessionRepository
    {
        Task<UserSession> GetByIdAsync(int id, CancellationToken ctoken);
        Task<UserSession> GetByUserIdAsync(int userId, CancellationToken ctoken);
        Task<IEnumerable<UserSession>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(UserSession userSession, CancellationToken ctoken);
        Task UpdateAsync(UserSession userSession);
        Task DeleteAsync(UserSession userSession);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
