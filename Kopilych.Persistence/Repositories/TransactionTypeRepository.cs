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
	public class TransactionTypeRepository : ITransactionTypeRepository
	{
		private readonly ApplicationDbContext _context;

		public TransactionTypeRepository(ApplicationDbContext context)
		{
			_context = context;
		}

        public async Task<TransactionType> GetByIdAsync(int id, CancellationToken ctoken)
        {
            return await _context.TransactionTypes.FindAsync(id, ctoken);
        }

        public async Task<IEnumerable<TransactionType>> GetAllAsync(CancellationToken ctoken)
        {
            return await _context.TransactionTypes.ToListAsync(ctoken);
        }

        public async Task AddAsync(TransactionType transactionType, CancellationToken ctoken)
        {
            await _context.TransactionTypes.AddAsync(transactionType, ctoken);
        }

        public async Task UpdateAsync(TransactionType transactionType)
        {
            _context.TransactionTypes.Update(transactionType);
        }

        public async Task DeleteAsync(TransactionType transactionType)
        {
            _context.TransactionTypes.Remove(transactionType);
        }

        public async Task SaveChangesAsync(CancellationToken ctoken)
        {
            // Сохраняет изменения в базе данных
            await _context.SaveChangesAsync(ctoken);
        }
    }

}
