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
	public class PiggyBankTypeRepository : IPiggyBankTypeRepository
	{
		private readonly ApplicationDbContext _context;

		public PiggyBankTypeRepository(ApplicationDbContext context)
		{
			_context = context;
		}

        public async Task<PiggyBankType> GetByIdAsync(int id, CancellationToken ctoken)
        {
            return await _context.PiggyBankTypes.FindAsync(id, ctoken);
        }

        public async Task<IEnumerable<PiggyBankType>> GetAllAsync(CancellationToken ctoken)
        {
            return await _context.PiggyBankTypes.ToListAsync(ctoken);
        }

        public async Task AddAsync(PiggyBankType piggyBankType, CancellationToken ctoken)
        {
            await _context.PiggyBankTypes.AddAsync(piggyBankType, ctoken);
        }

        public async Task UpdateAsync(PiggyBankType piggyBankType)
        {
            _context.PiggyBankTypes.Update(piggyBankType);
        }

        public async Task DeleteAsync(PiggyBankType piggyBankType)
        {
            _context.PiggyBankTypes.Remove(piggyBankType);
        }

        public async Task SaveChangesAsync(CancellationToken ctoken)
        {
            // Сохраняет изменения в базе данных
            await _context.SaveChangesAsync(ctoken);
        }
    }

}
