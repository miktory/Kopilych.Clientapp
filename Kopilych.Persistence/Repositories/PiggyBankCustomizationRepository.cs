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
    public class PiggyBankCustomizationRepository : IPiggyBankCustomizationRepository
    {
        private readonly ApplicationDbContext _context;

        public PiggyBankCustomizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken ctoken)
        {
            // Сохраняет изменения в базе данных
            await _context.SaveChangesAsync(ctoken);
        }

        public async Task<PiggyBankCustomization> GetByIdAsync(int id, CancellationToken ctoken)
        {
            return await _context.PiggyBankCustomizations.FindAsync(id, ctoken);
        }

        public async Task<IEnumerable<PiggyBankCustomization>> GetAllAsync(CancellationToken ctoken)
        {
            return await _context.PiggyBankCustomizations.ToListAsync(ctoken);
        }

        public async Task<IEnumerable<PiggyBankCustomization>> GetAllForPiggyBankAsync(int piggyBankId, CancellationToken ctoken)
        {
            return await _context.PiggyBankCustomizations.Where(c => c.PiggyBankId == piggyBankId).ToListAsync(ctoken);
        }

        public async Task<IEnumerable<PiggyBankCustomization>> GetAllForPiggyBankTypeAsync(int piggyBankTypeId, CancellationToken ctoken)
        {
            return await _context.PiggyBankCustomizations.Where(c => c.PiggyBankTypeId == piggyBankTypeId).ToListAsync(ctoken);
        }

        public async Task AddAsync(PiggyBankCustomization piggyBankCustomization, CancellationToken ctoken)
        {
            await _context.PiggyBankCustomizations.AddAsync(piggyBankCustomization);
        }

        public async Task UpdateAsync(PiggyBankCustomization piggyBankCustomization)
        {
            _context.PiggyBankCustomizations.Update(piggyBankCustomization);

        }

        public async Task DeleteAsync(PiggyBankCustomization piggyBankCustomization)
        {
            _context.PiggyBankCustomizations.Remove(piggyBankCustomization);

        }
    }
}
