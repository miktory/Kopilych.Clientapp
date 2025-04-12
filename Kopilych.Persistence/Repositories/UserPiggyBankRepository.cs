using Kopilych.Application.Interfaces.Repository;
using Kopilych.Domain;
using Kopilych.Shared.View_Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Persistence.Repositories
{
	public class UserPiggyBankRepository : IUserPiggyBankRepository
	{
		private readonly ApplicationDbContext _context;

		public UserPiggyBankRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<UserPiggyBank> GetByIdAsync(int id, CancellationToken ctoken)
		{
			// Получает UserPiggyBank по идентификатору
			return await _context.UserPiggyBanks.FindAsync(id, ctoken);
		}

		public async Task<IEnumerable<UserPiggyBank>> GetAllAsync(CancellationToken ctoken)
		{
			// Получает все UserPiggyBank
			return await _context.UserPiggyBanks.ToListAsync(ctoken);
		}

		public async Task<IEnumerable<UserPiggyBank>> GetAllForUserAsync(int userId, CancellationToken ctoken)
		{
			// Получает все UserPiggyBank для конкретного пользователя
			return await _context.UserPiggyBanks
				.Where(upb => upb.UserId == userId)
				.ToListAsync(ctoken);
		}

		public async Task<IEnumerable<UserPiggyBank>> GetAllForPiggyBankAsync(int piggyBankId, CancellationToken ctoken)
		{
			// Получает все UserPiggyBank для конкретной копилки
			return await _context.UserPiggyBanks
				.Where(upb => upb.PiggyBankId == piggyBankId)
				.ToListAsync(ctoken);
		}

		public async Task AddAsync(UserPiggyBank userPiggyBank, CancellationToken ctoken)
		{
			// Добавляет новую UserPiggyBank
			await _context.UserPiggyBanks.AddAsync(userPiggyBank, ctoken);
		}

		public async Task UpdateAsync(UserPiggyBank userPiggyBank)
		{
			// Обновляет существующий UserPiggyBank
			_context.UserPiggyBanks.Update(userPiggyBank);
		}

		public async Task DeleteAsync(UserPiggyBank userPiggyBank)
		{
			// Удаляет UserPiggyBank по идентификатору
			_context.UserPiggyBanks.Remove(userPiggyBank);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}

        public async Task<IEnumerable<int>> GetCommonPiggyBankIdsForUsersAsync(int firstUserId, int secondUserId, CancellationToken ctoken)
        {
            var coincidences =  await _context.UserPiggyBanks
				.Where(up => up.UserId == firstUserId || up.UserId == secondUserId)
				.GroupBy(up => up.PiggyBankId)
				.Where(g => g.Select(up => up.UserId).Distinct().Count() >= 2)
				.Select(g => g.Key)
				.ToListAsync();

            return coincidences;
        }

        public async Task<UserPiggyBank> GetByUserIdAndPiggyBankIdAsync(int userId, int piggyBankId, CancellationToken ctoken)
        {
			var result = await _context.UserPiggyBanks
				.Where(up => up.UserId == userId && up.PiggyBankId == piggyBankId).FirstOrDefaultAsync();

            return result;
        }
    }
}
