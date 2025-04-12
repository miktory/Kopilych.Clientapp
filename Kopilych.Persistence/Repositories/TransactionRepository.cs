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
	public class TransactionRepository : ITransactionRepository
	{
		private readonly ApplicationDbContext _context;

		public TransactionRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Transaction> GetByIdAsync(int id, CancellationToken ctoken)
		{
			return await _context.Transactions.FindAsync(id, ctoken);
		}

		public async Task<IEnumerable<Transaction>> GetAllAsync(CancellationToken ctoken)
		{
			return await _context.Transactions.ToListAsync(ctoken);
		}

        public async Task<IEnumerable<Transaction>> GetAllForUserAsync(int id, CancellationToken ctoken)
        {
            // Возвращает все PiggyBank
            return await _context.Transactions.Where(p => p.UserId == id).ToListAsync(ctoken);
        }

        public async Task AddAsync(Transaction transaction, CancellationToken ctoken)
		{
			// Добавляет новый PiggyBank
			await _context.Transactions.AddAsync(transaction, ctoken);
		}

		public async Task UpdateAsync(Transaction transaction)
		{
			// Обновляет существующий PiggyBank
			_context.Transactions.Update(transaction);
		}

		public async Task DeleteAsync(Transaction transaction)
		{
			// Удаляет PiggyBank по идентификатору
			_context.Transactions.Remove(transaction);
		}

		public async Task SaveChangesAsync(CancellationToken ctoken)
		{
			// Сохраняет изменения в базе данных
			await _context.SaveChangesAsync(ctoken);
		}

        public async Task<IEnumerable<Transaction>> GetAllForPiggyBankAsync(int piggyBankId, CancellationToken ctoken)
        {
			return await _context.Transactions.Where(p => p.PiggyBankId == piggyBankId).ToListAsync(ctoken);
        }
    }
}
