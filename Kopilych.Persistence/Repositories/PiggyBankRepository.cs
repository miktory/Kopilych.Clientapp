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
	public class PiggyBankRepository : IPiggyBankRepository
	{
		private readonly ApplicationDbContext _context;

		public PiggyBankRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<PiggyBank> GetByIdAsync(int id, CancellationToken ctoken)
		{
			// Возвращает PiggyBank по идентификатору
			return await _context.PiggyBanks.FindAsync(id, ctoken);
		}

		public async Task<IEnumerable<PiggyBank>> GetAllAsync(CancellationToken ctoken)
		{
			// Возвращает все PiggyBank
			return await _context.PiggyBanks.ToListAsync(ctoken);
		}

        public async Task<IEnumerable<PiggyBank>> GetAllForUserAsync(int id, CancellationToken ctoken)
        {
            // Возвращает все PiggyBank
            return await _context.PiggyBanks.Where(p => p.OwnerId == id).ToListAsync(ctoken);
        }

        public async Task AddAsync(PiggyBank userPiggyBank, CancellationToken ctoken)
		{
			// Добавляет новый PiggyBank
			await _context.PiggyBanks.AddAsync(userPiggyBank, ctoken);
		}

		public async Task UpdateAsync(PiggyBank userPiggyBank)
		{
			// Обновляет существующий PiggyBank
			_context.PiggyBanks.Update(userPiggyBank);
		}

		public async Task DeleteAsync(PiggyBank piggyBank)
		{
			// Удаляет PiggyBank по идентификатору
			_context.PiggyBanks.Remove(piggyBank);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}
	}
}
