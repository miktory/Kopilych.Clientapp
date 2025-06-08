
using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Middleware;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Kopilych.Mobile.View_Models
{
    public class TransactionViewModel : INotifyPropertyChanged
    {
        private TransactionTypeDTO _transactionType = new TransactionTypeDTO() {  };
        private PiggyBankDTO _piggyBank = new PiggyBankDTO() { };
        private PaymentTypeDTO _paymentType = new PaymentTypeDTO() { };
        private TransactionDTO _transaction = new TransactionDTO() { };
        private UserDetailsDTO _user = new UserDetailsDTO() {  };
        private ITransactionService _transactionService;
        private bool _remote;
        private IUserInfoService _userInfoService;
        private IFileService _fileService;
        private IMapper _mapper;
        private ICommand _markTransactionAsDeletedCommand;
        private bool _isExpanded;
        private bool _isDeleteAvailable;
        private ICommand _changeCardSizeCommand;
        private ImageSource _userImageSource;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string OperationDescription { get => $"{TransactionType}"+ " " + $"({PaymentType.ToLower()})"; private set { } }
        public string Username { get => _user.Username; private set { } }
        public string Amount { get => (_transaction.Amount >=0 ? "+" : "") + _transaction.Amount.ToString(); private set { } }
        public bool IsPositiveAmount { get => _transactionType.IsPositive; private set { } }
        public string Date { get => _transaction.Date.ToLocalTime().ToShortDateString(); private set { } }
        public bool IsVisible { get => _transaction.IsDeleted.HasValue ? !_transaction.IsDeleted.Value : true; private set { } }
        public bool IsDeleteAvailable { get => _isDeleteAvailable; }
        public bool IsLoaded { get; private set; }

        public bool IsExpanded { get => _isExpanded; private set { _isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); } }

        public string Description { get => _transaction.Description; private set { } }
        public string TransactionType { get => _transactionType.Name; private set { } }
        public string PaymentType { get => _paymentType.Name; private set { } }
        public ImageSource UserImageSource { get => _userImageSource; private set { _userImageSource = value; OnPropertyChanged(nameof(UserImageSource)); } }
        public string UserPhotoPath { get => string.IsNullOrEmpty(_user.PhotoPath) ? DefaultPaths.UserImage : _user.PhotoPath; private set { } }

        public ICommand MarkTransactionAsDeletedCommand { get => _markTransactionAsDeletedCommand; private set { _markTransactionAsDeletedCommand = value; OnPropertyChanged(nameof(MarkTransactionAsDeletedCommand)); } }
        public ICommand ChangeCardSizeCommand { get => _changeCardSizeCommand; private set { _changeCardSizeCommand = value; OnPropertyChanged(nameof(ChangeCardSizeCommand)); } }

        public TransactionViewModel() { }

        public TransactionViewModel(TransactionDTO transaction, TransactionTypeDTO transactionType, PaymentTypeDTO paymentType, UserDetailsDTO userDetails, PiggyBankDTO piggyBank, ITransactionService transactionService, IUserInfoService userInfoService, IMapper mapper, IFileService fileService, bool isDeleteAvailable, bool runInit = false, bool remote = false)
        {
            UserImageSource = DefaultPaths.UserImage;
            _transaction = transaction;
            _piggyBank = piggyBank;
            _user = userDetails;
            _paymentType = paymentType;
            _transactionType = transactionType;
            _mapper = mapper;
            _transactionService = transactionService;
            _remote = remote;
            _userInfoService = userInfoService;
            _fileService = fileService;
            _isDeleteAvailable = isDeleteAvailable;
            if (runInit)
                Init();
        }

        public void Init()
        {
            MarkTransactionAsDeletedCommand = new Command(async () => {
                try
                {
                    await DisplayDeleteTransactionPopupAsync();
                } catch (Exception ex) 
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
            ChangeCardSizeCommand = new Command(() => IsExpanded = !IsExpanded);
        }

        public void ChangeDisplayMode(bool remote)
        {
            _remote = remote;
        }

        public async Task LoadData()
        {
            await LoadImage(_remote);
        }

        private async Task LoadImage(bool remote)
        {
            try
            {
                if (!remote)
                {
                    if (_fileService.Exist(UserPhotoPath))
                        UserImageSource = ImageSource.FromFile(UserPhotoPath);
                    else
                        UserImageSource = ImageSource.FromFile(DefaultPaths.UserImage);
                }
                else
                {

                    byte[] image = null;

                    image = await _userInfoService.GetUserPhotoAsync(_user, CancellationToken.None, true);
                    UserImageSource = ImageSource.FromStream(() => new MemoryStream(image));


                }
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    UserImageSource = ImageSource.FromFile(DefaultPaths.UserImage);
                else
                    CustomExceptionHandlerMiddleware.Handle(ex);
            }
        }


        private async Task DisplayDeleteTransactionPopupAsync()
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить транзакцию?", "Да", "Нет");
            if (answer)
            {
                if (!_remote)
                {
                    var updateDto = _mapper.Map<UpdateTransactionDTO>(_transaction);
                    updateDto.IsDeleted = true;
                    updateDto.Amount = 0;
                    await _transactionService.UpdateTransactionAsync(_transaction.Id, updateDto, CancellationToken.None, false);
                    _transaction.Amount = 0;
                    _transaction.IsDeleted = true;
                }
                else
                {
                    await _transactionService.DeleteTransactionAsync(_transaction.Id, CancellationToken.None, true);
                }
                OnPropertyChanged(nameof(IsVisible));

                OnPropertyChanged(nameof(Amount));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
