using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IUserFriendshipRepository
    {
        Task<UserFriendship> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<UserFriendship>> GetAllAsync(CancellationToken ctoken);
        Task<IEnumerable<UserFriendship>> GetAllForUserAsync(int userId, CancellationToken ctoken);
        Task AddAsync(UserFriendship userFriendship, CancellationToken ctoken);
        Task UpdateAsync(UserFriendship userFriendship);
        Task DeleteAsync(UserFriendship userFriendship);
		Task SaveChangesAsync(CancellationToken ctoken);
        Task<UserFriendship> GetBySpecifiedUserIdsAsync(int firstUserId, int secondUserId, CancellationToken ctoken);


    }
}
