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
	public class PaymentTypeRepository : IPaymentTypeRepository
	{
		private readonly ApplicationDbContext _context;

		public PaymentTypeRepository(ApplicationDbContext context)
		{
			_context = context;
		}

        public async Task<PaymentType> GetByIdAsync(int id, CancellationToken ctoken)
        {
            return await _context.PaymentTypes.FindAsync(id, ctoken);
        }

        public async Task<IEnumerable<PaymentType>> GetAllAsync(CancellationToken ctoken)
        {
            return await _context.PaymentTypes.ToListAsync(ctoken);
        }

        public async Task AddAsync(PaymentType paymentType, CancellationToken ctoken)
        {
            await _context.PaymentTypes.AddAsync(paymentType, ctoken);
        }

        public async Task UpdateAsync(PaymentType paymentType)
        {
            _context.PaymentTypes.Update(paymentType);
        }

        public async Task DeleteAsync(PaymentType paymentType)
        {
            _context.PaymentTypes.Remove(paymentType);
        }

        public async Task SaveChangesAsync(CancellationToken ctoken)
        {
            // Сохраняет изменения в базе данных
            await _context.SaveChangesAsync(ctoken);
        }
    }

}
