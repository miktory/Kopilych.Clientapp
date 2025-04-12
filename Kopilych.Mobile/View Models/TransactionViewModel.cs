using Kopilych.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = Kopilych.Mobile.Models.Transaction;

namespace Kopilych.Mobile.View_Models
{
    public class TransactionViewModel
    {
        private TransactionType _transactionType = new Models.TransactionType() {  };
        private PaymentType _paymentType = new Models.PaymentType() { };
        private Transaction _transaction = new Transaction() { };
        private User _user = new User() {  };

        public string OperationDescription { get => $"{TransactionType}"+ $"({PaymentType})"; private set { } }
        public string Username { get => _user.Username; private set { } }
        public decimal Amount { get => _transaction.Amount; private set { } }
        public DateTime Date { get => _transaction.Date; private set { } }

        public bool IsLoaded { get; private set; }

        public string Description { get => _transaction.Description; private set { } }
        public string TransactionType { get => _transactionType.Name; private set { } }
        public string PaymentType { get => _paymentType.Name; private set { } }
        public string UserPhotoPath { get => _user.PhotoPath; private set { } }

        public TransactionViewModel() { }

        public TransactionViewModel(Transaction transaction)
        {
            _transaction = transaction;
        }
    }
}
