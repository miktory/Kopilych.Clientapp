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
	public class PremiumUserRepository : IPremiumUserRepository
	{
		private readonly ApplicationDbContext _context;

		public PremiumUserRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<PremiumUser> GetByIdAsync(int id, CancellationToken ctoken)
		{
			return await _context.PremiumUsers.FindAsync(id, ctoken);
		}

		public async Task<IEnumerable<PremiumUser>> GetAllAsync(CancellationToken ctoken)
		{
			// Получает все UserFriendship
			return await _context.PremiumUsers.ToListAsync(ctoken);
		}

		public async Task<PremiumUser> GetForUserAsync(int userId, CancellationToken ctoken)
		{
			// Получает все дружбы для конкретного пользователя
			return await _context.PremiumUsers
				.Where(pu => pu.UserId == userId)
				.FirstOrDefaultAsync(ctoken);
		}

        public async Task AddAsync(PremiumUser premiumUser, CancellationToken ctoken)
		{
			// Добавляет новую дружбу
			await _context.PremiumUsers.AddAsync(premiumUser, ctoken);
		}

		public async Task UpdateAsync(PremiumUser premiumUser)
		{
			// Обновляет существующую дружбу
			_context.PremiumUsers.Update(premiumUser);
		}

		public async Task DeleteAsync(PremiumUser premiumUser)
		{
			// Удаляет дружбу по идентификатору
				_context.PremiumUsers.Remove(premiumUser);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}
	}

}
