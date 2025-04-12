using Kopilych.Application.Interfaces.Repository;
using Kopilych.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence.Repositories
{
	public class UserFriendshipRepository : IUserFriendshipRepository
	{
		private readonly ApplicationDbContext _context;

		public UserFriendshipRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<UserFriendship> GetByIdAsync(int id, CancellationToken ctoken)
		{
			// Получает UserFriendship по идентификатору
			return await _context.UserFriendships.FindAsync(id, ctoken);
		}

		public async Task<IEnumerable<UserFriendship>> GetAllAsync(CancellationToken ctoken)
		{
			// Получает все UserFriendship
			return await _context.UserFriendships.ToListAsync(ctoken);
		}

		public async Task<IEnumerable<UserFriendship>> GetAllForUserAsync(int userId, CancellationToken ctoken)
		{
			// Получает все дружбы для конкретного пользователя
			return await _context.UserFriendships
				.Where(uf => uf.InitiatorUserId == userId || uf.ApproverUserId == userId)
				.ToListAsync(ctoken);
		}

        public async Task<UserFriendship> GetBySpecifiedUserIdsAsync(int firstUserId, int secondUserId, CancellationToken ctoken)
        {
            // Получает все дружбы для конкретного пользователя
            return await _context.UserFriendships
                .Where(uf => (uf.InitiatorUserId == firstUserId && uf.ApproverUserId == secondUserId) || (uf.InitiatorUserId == secondUserId && uf.ApproverUserId == firstUserId)).FirstOrDefaultAsync(ctoken);
        }

        public async Task AddAsync(UserFriendship userFriendship, CancellationToken ctoken)
		{
			// Добавляет новую дружбу
			await _context.UserFriendships.AddAsync(userFriendship, ctoken);
		}

		public async Task UpdateAsync(UserFriendship userFriendship)
		{
			// Обновляет существующую дружбу
			_context.UserFriendships.Update(userFriendship);
		}

		public async Task DeleteAsync(UserFriendship userFriendship)
		{
			// Удаляет дружбу по идентификатору
				_context.UserFriendships.Remove(userFriendship);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}
	}

}
