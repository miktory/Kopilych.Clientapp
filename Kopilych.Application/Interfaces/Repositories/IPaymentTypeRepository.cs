using Kopilych.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces.Repository
{
    public interface IPaymentTypeRepository
    {
        Task<PaymentType> GetByIdAsync(int id, CancellationToken ctoken);
        Task<IEnumerable<PaymentType>> GetAllAsync(CancellationToken ctoken);
        Task AddAsync(PaymentType paymentType, CancellationToken ctoken);
        Task UpdateAsync(PaymentType paymentType);
        Task DeleteAsync(PaymentType paymentType);
		Task SaveChangesAsync(CancellationToken ctoken);
	}
}
