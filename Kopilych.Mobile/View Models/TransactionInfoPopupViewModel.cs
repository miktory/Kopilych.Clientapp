using Kopilych.Domain;
using Kopilych.Shared.DTO;
using Kopilych.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Shared.View_Models;
using System.Reflection;
using System.Collections.ObjectModel;
using Kopilych.Persistence.Migrations;
using CommunityToolkit.Maui.Core;
using Kopilych.Application.Interfaces;
using Kopilych.Mobile.Middleware;
using CommunityToolkit.Maui.Core.Extensions;
using AutoMapper;
using System.Windows.Input;
using System.Runtime.CompilerServices;

namespace Kopilych.Mobile.View_Models
{
    public class TransactionInfoPopupViewModel : INotifyPropertyChanged
    {
        private IPopupService _popupService;
        private IUserInfoService _userInfoService;
        private ITransactionService _transactionService;
        private IMapper _mapper;
        private bool _remote;
        private PaymentTypeDTO _paymentType;
        private TransactionTypeDTO _transactionType;
        private TransactionDTO _transaction;
        private ObservableCollection<PaymentTypeDTO> _paymentTypes;
        private ObservableCollection<TransactionTypeDTO> _transactionTypes;

        public TransactionDTO Transaction { get => _transaction; private set { _transaction = value; OnPropertyChanged(nameof(Transaction)); } }
        public PaymentTypeDTO PaymentType { get => _paymentType; set { _paymentType = value; OnPropertyChanged(nameof(PaymentType)); } }
        public TransactionTypeDTO TransactionType { get => _transactionType; set { _transactionType = value; OnPropertyChanged(nameof(TransactionType)); } }
        public DateTime Date { get => _transaction == null ? DateTime.Now : _transaction.Date.ToLocalTime(); set { _transaction.Date.ToUniversalTime(); OnPropertyChanged(nameof(_transaction.Date)); } }
        public ObservableCollection<TransactionTypeDTO> TransactionTypes { get => _transactionTypes; private set {_transactionTypes = value; OnPropertyChanged(nameof(TransactionTypes)); } } 
        public ObservableCollection<PaymentTypeDTO> PaymentTypes { get => _paymentTypes; private set { _paymentTypes = value; OnPropertyChanged(nameof(PaymentTypes)); } }
        public ICommand CloseWithNoChangesCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }
        public Command FlushDescriptionCommand { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public TransactionInfoPopupViewModel(IPopupService popupService, IUserInfoService userInfoService, ITransactionService transactionService, IMapper mapper, bool remote = false, TransactionDTO? transaction = null, List<PaymentTypeDTO>? paymentTypes = null, List<TransactionTypeDTO>? transactionTypes = null)
        {
            _mapper = mapper;
            Transaction = transaction;
            _transactionService = transactionService;
            _popupService = popupService;
            _userInfoService = userInfoService;
            _remote = remote;
            if (transactionTypes != null)
                TransactionTypes = transactionTypes.ToObservableCollection();
            if (paymentTypes != null)
                PaymentTypes = paymentTypes.ToObservableCollection();
            if (Transaction == null)
            {
                Transaction = new TransactionDTO();
                Transaction.Date = DateTime.UtcNow;
            }
            Init();
        }

        public TransactionInfoPopupViewModel()
        {

        }

        private async void Init()
        {

                CloseWithNoChangesCommand = new Command(async () => { await OnDiscardChanges(); });
                SaveChangesCommand = new Command(async () =>
                {
                    try
                    {
                        await SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        CustomExceptionHandlerMiddleware.Handle(ex);
                    }
                });
                FlushDescriptionCommand = new Command(() => { Transaction.Description = null; OnPropertyChanged(nameof(Transaction)); });
                if (TransactionTypes == null)
                    TransactionTypes = new ObservableCollection<TransactionTypeDTO>() { };
            if (PaymentTypes == null)
                PaymentTypes = new ObservableCollection<PaymentTypeDTO>() { };
            TransactionType = new TransactionTypeDTO();
                PaymentType = new PaymentTypeDTO();
                PaymentType = PaymentTypes.Where(x => x.Id == Transaction.PaymentTypeId).FirstOrDefault() ?? PaymentTypes.FirstOrDefault();
                TransactionType = TransactionTypes.Where(x => x.Id == Transaction.TransactionTypeId).FirstOrDefault() ?? TransactionTypes.FirstOrDefault();
            UpdateAllPropertiesUI();



        }

        private void UpdateAllPropertiesUI()
        {
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Проходим по каждому свойству и вызываем OnPropertyChanged
            foreach (var property in properties)
            {
                OnPropertyChanged(property.Name);
            }
        }

        public async Task LoadDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                PaymentTypes = (await _transactionService.GetAllPaymentTypesAsync(cancellationToken)).ToObservableCollection();
                TransactionTypes = (await _transactionService.GetAllTransactionTypesAsync(cancellationToken)).ToObservableCollection();
                PaymentType = PaymentTypes.Where(x => x.Id == Transaction.PaymentTypeId).FirstOrDefault() ?? PaymentTypes.FirstOrDefault(); ;
                TransactionType = TransactionTypes.Where(x => x.Id == Transaction.TransactionTypeId).FirstOrDefault() ?? TransactionTypes.FirstOrDefault();
            }
            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);
            }
        }

        public async Task LoadFromDTO(TransactionDTO transaction, List<TransactionTypeDTO> transactionTypes, List<PaymentTypeDTO> paymentTypes)
        {
            Transaction = transaction;
            PaymentTypes = paymentTypes.ToObservableCollection();
            TransactionTypes = transactionTypes.ToObservableCollection();
            PaymentType = PaymentTypes.Where(x => x.Id == Transaction.PaymentTypeId).FirstOrDefault() ?? PaymentTypes.FirstOrDefault(); ;
            TransactionType = TransactionTypes.Where(x => x.Id == Transaction.TransactionTypeId).FirstOrDefault() ?? TransactionTypes.FirstOrDefault();

        }

        public void ChangeDisplayMode(bool remote)
        {
            _remote = remote;
        }

        private async Task<int> CreateTransaction()
        {
            var amountAbs = Transaction.Amount < 0 ? Transaction.Amount * -1 : Transaction.Amount;
            Transaction.PaymentTypeId = PaymentType.Id;
            Transaction.TransactionTypeId = TransactionType.Id;
            if (!TransactionType.IsPositive)
                amountAbs *= -1;
            Transaction.Amount = amountAbs;
            Transaction.UserId = (await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, _remote)).Id;
            var transaction = _mapper.Map<CreateTransactionDTO>(Transaction);
            var id = await _transactionService.CreateTransactionAsync(transaction, CancellationToken.None, _remote);
            Transaction.Id = id;
            
            return id;
        }

        async Task OnSave()
        {
            await _popupService.ClosePopupAsync(Transaction);
        }

        async Task OnDiscardChanges()
        {
            await _popupService.ClosePopupAsync();
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                int? id = null;
                id = await CreateTransaction();
                if (id != null)
                        await OnSave();
            }

            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);

            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
