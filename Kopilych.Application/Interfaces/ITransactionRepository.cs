using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
    public interface ITransactionService
    {
        public Task<TransactionTypeDTO> GetTransactionTypeDetailsAsync(int transactionTypeId, CancellationToken cancellationToken);
        public Task<PaymentTypeDTO> GetPaymentTypeDetailsAsync(int paymentTypeId, CancellationToken cancellationToken);
    }
}
