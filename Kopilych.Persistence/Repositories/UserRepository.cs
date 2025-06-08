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
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;

		public UserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<User> GetByIdAsync(int id, CancellationToken ctoken)
		{
			// Получает пользователя по идентификатору
			return await _context.Users.FindAsync(id, ctoken);
		}

		public async Task<User> GetByExternalIdAsync(int id, CancellationToken ctoken)
		{
			// Получает пользователя по GUID
			return await _context.Users.FirstOrDefaultAsync(u => u.ExternalId == id, ctoken);
		}

		public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ctoken)
		{
			// Получает всех пользователей
			return await _context.Users.ToListAsync(ctoken);
		}

		public async Task AddAsync(User user, CancellationToken ctoken)
		{
			// Добавляет нового пользователя
			await _context.Users.AddAsync(user, ctoken);
		}

		public async Task UpdateAsync(User user)
		{
			// Обновляет существующего пользователя
			_context.Users.Update(user);
		}

		public async Task DeleteAsync(User user)
		{
			// Удаляет пользователя по идентификатору
				_context.Users.Remove(user);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}
	}
}
