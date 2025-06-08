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
	public class UserSessionRepository : IUserSessionRepository
	{
		private readonly ApplicationDbContext _context;

		public UserSessionRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<UserSession> GetByIdAsync(int id, CancellationToken ctoken)
		{

			return await _context.UserSessions.FindAsync(id, ctoken);
		}

		public async Task<UserSession> GetByUserIdAsync(int userId, CancellationToken ctoken)
		{

			return await _context.UserSessions.FirstOrDefaultAsync(u => u.UserId == userId, ctoken);
		}

		public async Task<IEnumerable<UserSession>> GetAllAsync(CancellationToken ctoken)
		{
			// Получает всех пользователей
			return await _context.UserSessions.ToListAsync(ctoken);
		}

		public async Task AddAsync(UserSession userSession, CancellationToken ctoken)
		{
			// Добавляет нового пользователя
			await _context.UserSessions.AddAsync(userSession, ctoken);
		}

		public async Task UpdateAsync(UserSession userSession)
		{
			// Обновляет существующего пользователя
			_context.UserSessions.Update(userSession);
		}

		public async Task DeleteAsync(UserSession userSession)
		{
			// Удаляет пользователя по идентификатору
				_context.UserSessions.Remove(userSession);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}
	}
}
