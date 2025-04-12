using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Mobile.Models;

namespace Kopilych.Mobile.View_Models
{
    public  class PiggyBankPageViewModel : INotifyPropertyChanged
    {
        private PiggyBank _piggyBank;


        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTransactionsLoaded { get; private set; }

        public ObservableCollection<TransactionViewModel> TransactionsViewModels { get; private set; }
        public ObservableCollection<Transaction> Transactions { get; private set; }

        public PiggyBankPageViewModel() { }

        public PiggyBankPageViewModel(PiggyBank piggyBank)
        {
            _piggyBank = piggyBank;

        }


        public async Task LoadTransactionsAsync()
        {
            // вызываем в OnAppearing()
            IsTransactionsLoaded = false;
            Transactions = await GetTransactionsAsync();
            OnPropertyChanged(nameof(Transactions));
            if (TransactionsViewModels == null)
                TransactionsViewModels = new ObservableCollection<TransactionViewModel>();
            foreach (var t in Transactions)
                TransactionsViewModels.Add(new TransactionViewModel(t));
            OnPropertyChanged(nameof(TransactionsViewModels));
            IsTransactionsLoaded = true;
            OnPropertyChanged(nameof(IsTransactionsLoaded));
        }

        private async Task<ObservableCollection<Transaction>> GetTransactionsAsync()
        {
            // реализовать логику

            var result = new ObservableCollection<Transaction>() { new Transaction() { Amount = 1000, Created = DateTime.UtcNow, 
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 },
            new Transaction() { Amount = 1000, Created = DateTime.UtcNow,
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 },
            new Transaction() { Amount = 1000, Created = DateTime.UtcNow,
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 },
            new Transaction() { Amount = 1000, Created = DateTime.UtcNow,
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 },
            new Transaction() { Amount = 1000, Created = DateTime.UtcNow,
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 },
            new Transaction() { Amount = 1000, Created = DateTime.UtcNow,
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 },
            new Transaction() { Amount = 1000, Created = DateTime.UtcNow,
                Date = DateTime.UtcNow, Updated = DateTime.UtcNow, Description = "Описание", UserId = 123, ExternalId = 787898, Id = 5, PaymentTypeId = 1, PiggyBankId = 1, TransactionTypeId = 1 }};
            return result;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
