using Kopilych.Application.Services;
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
        public Task<List<PaymentTypeDTO>> GetAllPaymentTypesAsync(CancellationToken cancellationToken);
        public Task<List<TransactionTypeDTO>> GetAllTransactionTypesAsync(CancellationToken cancellationToken);

        public Task<TransactionDTO> GetTransactionDetailsAsync(int transactionId, CancellationToken cancellationToken);
        public Task<List<TransactionDTO>> GetTransactionsByPiggyBankIdAsync(int piggyBankId, CancellationToken cancellationToken, bool remote);
        public Task<int> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO, CancellationToken cancellationToken, bool remote, bool updatePiggyBankBalance = true);
        public Task DeleteTransactionAsync(int transactionId, CancellationToken cancellationToken, bool remote, bool updatePiggyBankBalance = true);
        public Task UpdateTransactionAsync(int transactionId, UpdateTransactionDTO updateTransactionDTO, CancellationToken cancellationToken, bool remote, bool updatePiggyBankBalance = true);
        public Task<TransactionDTO> RunTwoWayTransactionIntegration(TransactionDTO? localTransaction, TransactionDTO? externalTransaction, CancellationToken cancellationToken);
    }
}
