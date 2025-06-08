using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.AdServices.AdIds;
using Android.OS;
using Android.Text;
using AutoMapper;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Mobile.Views;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;

namespace Kopilych.Mobile.View_Models
{
    public class PiggyBankPageViewModel : INotifyPropertyChanged
    {
        private bool _isExpanded;
        private IPiggyBankService _piggyBankService;
        private ITransactionService _transactionService;
        private INavigationService _navigationService;
        private IUserInfoService _userInfoService;
        private IPopupService _popupService;
        private IMapper _mapper;
        private IFileService _fileService;
        private bool _remote;
        private BackButtonViewModel _backButtonViewModel;
        private ImageSource _piggyBankImageSource;
        private bool _isRefreshing;

        public PiggyBankDTO PiggyBank { get; private set; } = new PiggyBankDTO();
        public string PiggyBankTypePhotoPath
        {
            get
            {
                if (!PiggyBank.Goal.HasValue)
                    return PiggyBankType.FirstStatePhotoPath;

                if (PiggyBank.Balance <= 0)
                    return PiggyBankType.FirstStatePhotoPath;
                else if (PiggyBank.Balance / PiggyBank.Goal * 100 <= 25)
                    return PiggyBankType.SecondStatePhotoPath;
                else if (PiggyBank.Balance / PiggyBank.Goal * 100 <= 75)
                    return PiggyBankType.ThirdStatePhotoPath;
                else
                    return PiggyBankType.FourthStatePhotoPath;
            }
        }

        public string PhotoPath
        {
            get
            {
                return string.IsNullOrEmpty(PiggyBankCustomization.PhotoPath) ? DefaultPaths.PiggyBankImage : PiggyBankCustomization.PhotoPath;
            }
        }

        public string GoalDate 
        { 
            get 
            { 
                return PiggyBank.GoalDate.HasValue ? PiggyBank.GoalDate.Value.ToLocalTime().ToShortDateString() : "-";
            } 
        }

        public PiggyBankCustomizationDTO PiggyBankCustomization { get; private set; } = new PiggyBankCustomizationDTO();
        public PiggyBankTypeDTO PiggyBankType { get; private set; } = new PiggyBankTypeDTO();
        public UserPiggyBankDTO? UserPiggyBank { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BackButtonViewModel BackButtonViewModel { get { return _backButtonViewModel; } set { _backButtonViewModel = value; OnPropertyChanged(nameof(BackButtonViewModel)); } }
        public bool IsPiggyBankDeleted { get => PiggyBank.IsDeleted.HasValue ? PiggyBank.IsDeleted.Value : false; }
        public bool IsRemote { get => _remote; }
        public bool IsTransactionsLoaded { get; private set; }
        public bool IsEditMode { get; private set; }
        public bool IsMember { get => UserPiggyBank != null; }
        public bool CanOpenPrivacyMenu { get => UserPiggyBank != null && !PiggyBank.Shared; } // тут не проверяем, относится ли ссылка к current user, смотрим только наличие. надо добавить проверку
        public bool CanOpenMembersMenu { get => UserPiggyBank != null && PiggyBank.Shared; } // тут не проверяем, относится ли ссылка к current user, смотрим только наличие. надо добавить проверку
        public bool CanCreateTransactions { get => UserPiggyBank != null; }
        public bool IsRefreshing { get => _isRefreshing; set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }

        public bool IsExpanded { get => _isExpanded; set { _isExpanded = value; OnPropertyChanged(nameof(IsExpanded)); } }

        public ObservableCollection<TransactionViewModel> TransactionsViewModels { get; private set; }
        public ObservableCollection<TransactionDTO> Transactions { get; private set; }
        public ICommand ChangeCardExpandStatusCommand { get; private set; }
        public ICommand MarkPiggyBankAsDeletedCommand { get; private set; }
        public ICommand EditPiggyBankCommand { get; private set; }
        public ICommand CreateTransactionCommand { get; private set; }
        public ICommand OpenMembersMenuCommand { get; private set; }
        public ICommand OpenPrivacyMenuCommand { get; private set; }
        public ICommand RefreshDataCommand { get; private set; }
        public ICommand OpenPhotoCommand { get; private set; }

        public ImageSource PiggyBankImageSource { get => _piggyBankImageSource; private set { _piggyBankImageSource = value; OnPropertyChanged(nameof(PiggyBankImageSource)); } }

        public PiggyBankPageViewModel() { }

        public PiggyBankPageViewModel(BackButtonViewModel backButtonVm, UserPiggyBankDTO? userPiggyBank, PiggyBankDTO piggyBank, PiggyBankCustomizationDTO piggyBankCustomization, IUserInfoService userInfoService, IPiggyBankService piggyBankService, IPopupService popupService, ITransactionService transactionService, IMapper mapper, INavigationService navigationService, IFileService fileService, bool remote)
        {
            PiggyBank = piggyBank;
            PiggyBankCustomization = piggyBankCustomization;
            UserPiggyBank = userPiggyBank;
            _userInfoService = userInfoService;
            _piggyBankService = piggyBankService;
            _popupService = popupService;
            _mapper = mapper;
            _transactionService = transactionService;
            _navigationService = navigationService;
            _remote = remote;
            _fileService = fileService;
            BackButtonViewModel = backButtonVm;
            Init();

        }

        public double FillLevel
        {
            get => PiggyBank.Percentage / 100.0;
            private set { }
        }

        public string? GoalBalance
        {
            get => PiggyBank.Goal.HasValue ? PiggyBank.Goal.Value.ToString() : "∞";
            private set { }
        }

        public void Init()
        {
            EditPiggyBankCommand = new Command(async () => { await DisplayEditPiggyBankPopupAsync(); });
            ChangeCardExpandStatusCommand = new Command(async () => { IsExpanded = !IsExpanded; });
            RefreshDataCommand = new Command(async () => { IsRefreshing = true; await LoadDataAsync(); IsRefreshing = false; });
            MarkPiggyBankAsDeletedCommand = new Command<PiggyBankDTO>(async (PiggyBankDTO piggybank) => {
                try
                {
                   var ans = await DisplayDeletePiggyBankPopupAsync(piggybank);
                    if (ans)
                        await _navigationService.GoBackAsync();
                } catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
            CreateTransactionCommand = new Command(async () => {
                if (CanCreateTransactions)
                    await DisplayCreateTransactionPopupAsync(); });



            OpenMembersMenuCommand = new Command(async () => {
                using (var cts = new CancellationTokenSource())
                {
                    _popupService.ShowPopup<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cts.Token));
                    PiggyBankMembersPageViewModel membersPageVm = null;
                    PiggyBankMembersPageView membersPageView = null;
                    await Task.Run(async () =>
                    {
                        var currentUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false);
                        membersPageVm = Microsoft.Maui.MauiApplication.Current.Services.GetService<PiggyBankMembersPageViewModel>();
                        membersPageVm.LoadFromDTO(PiggyBank, currentUser);
                        membersPageView = new PiggyBankMembersPageView(membersPageVm);
                        membersPageView.BindingContext = membersPageVm;
                    

                    });
                    await _navigationService.NavigateToAsync(membersPageView);
                    cts.Cancel();
                }

            });
            OpenPrivacyMenuCommand = new Command(async () => {
                try
                {
                    await DisplayEditPrivacyPopupAsync();
                } catch (Exception ex)
                { CustomExceptionHandlerMiddleware.Handle(ex); }
            });
            // разделение логики для групповой и персональной копилок
            OpenPhotoCommand = new Command(async () => { await DisplayImagePopupAsync(PiggyBankImageSource); });



        }

        private async Task DisplayImagePopupAsync(ImageSource imageSource)
        {
            await _popupService.ShowPopupAsync<ImagePopupViewModel>(vm => vm.LoadImage(imageSource));
        }


        public async Task ConfigureUiAsync()
        {
           await Task.Run(async () =>
            {
                try
                {
                    var currentUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false);
                    if (PiggyBank.Shared)
                    {
                        if (!currentUser.ExternalId.HasValue)
                            ChangeDisplayMode(false, this.IsRemote);
                        else
                            ChangeDisplayMode(PiggyBank.OwnerId == currentUser.ExternalId, this.IsRemote);
                    }
                    else
                    {
                        if (PiggyBank.OwnerId == currentUser.Id)
                            ChangeDisplayMode(true, this.IsRemote);
                    }
                    UpdateAllPropertiesUI();

                }

                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
        }


        private async Task LoadImageAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (!_remote)
                    {
                        if (_fileService.Exist(PhotoPath))
                            PiggyBankImageSource = ImageSource.FromFile(PhotoPath);
                        else
                            PiggyBankImageSource = ImageSource.FromFile(DefaultPaths.PiggyBankImage);
                    }
                    else
                    {
                        byte[] image = null;

                        image = await _piggyBankService.GetPiggyBankCustomizationPhotoAsync(PiggyBankCustomization, CancellationToken.None, true);
                        PiggyBankImageSource = ImageSource.FromStream(() => new MemoryStream(image));


                    }
                }
                catch (Exception ex)
                {
                    if (ex is NotFoundException)
                        PiggyBankImageSource = ImageSource.FromFile(DefaultPaths.PiggyBankImage);
                    else
                        CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
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
        private async Task DisplayEditPiggyBankPopupAsync()
        {
            await _popupService.ShowPopupAsync<PiggyBankInfoPopupViewModel>(vm => { vm.LoadFromDTO(PiggyBank, PiggyBankCustomization); vm.ChangeDisplayMode(true, false); });
            await LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
                using (var cts = new CancellationTokenSource())
                {
                    _popupService.ShowPopup<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cts.Token));
                    await Task.Run(async () =>
                    {
                        try
                        {
                            await LoadImageAsync();
                            await LoadPiggyBankInfoAsync();
                            if (IsMember)
                                await LoadTransactionsAsync();

                        }
                        catch (Exception ex)
                        {
                            CustomExceptionHandlerMiddleware.Handle(ex);
                        }
                        OnPropertyChanged(nameof(GoalDate));
                    });
                    cts.Cancel();
                }
        }

        public void LoadFromDTO(PiggyBankDTO piggyBank, PiggyBankCustomizationDTO piggyBankCustomization, UserPiggyBankDTO? userPiggyBank)
        {
            PiggyBank = piggyBank;
            PiggyBankCustomization = piggyBankCustomization;
            UserPiggyBank = userPiggyBank;

            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Проходим по каждому свойству и вызываем OnPropertyChanged
            foreach (var property in properties)
            {
                OnPropertyChanged(property.Name);
            }
          //  OnPropertyChanged(nameof(_userDetails));
        }

        public void ChangeDisplayMode(bool isEditMode, bool remote)
        {
            _remote = remote;
            OnPropertyChanged(nameof(IsRemote));
            IsEditMode = isEditMode;
            OnPropertyChanged(nameof(IsEditMode));
        }


        private async Task LoadTransactionsAsync()
        {
            await Task.Run(async () =>
            {
                IsTransactionsLoaded = false;
                if (TransactionsViewModels == null)
                    TransactionsViewModels = new ObservableCollection<TransactionViewModel>();
                foreach (var t in TransactionsViewModels)
                {
                    try
                    {
                        t.PropertyChanged -= OnCollectionItemPropertyChanged;
                    }
                    catch (Exception ex) { }
                }

                TransactionsViewModels.Clear();
                OnPropertyChanged(nameof(TransactionsViewModels));
                var paymentTypes = await GetPaymentTypesAsync();
                var transactionTypes = await GetTransactionTypesAsync();
                Transactions = await GetTransactionsAsync();
                OnPropertyChanged(nameof(Transactions));
                Transactions = Transactions.OrderByDescending(x => x.Id).ToObservableCollection();
                foreach (var t in Transactions)
                {
                    var user = await GetUserDetailsAsync(t.UserId);
                    var vm = new TransactionViewModel(t, transactionTypes.FirstOrDefault(x => x.Id == t.TransactionTypeId), paymentTypes.FirstOrDefault(x => x.Id == t.PaymentTypeId), user, PiggyBank, _transactionService, _userInfoService, _mapper, _fileService, UserPiggyBank != null && UserPiggyBank.UserId == t.UserId || PiggyBank.OwnerId == UserPiggyBank.UserId, false, _remote);
                    if (!t.IsDeleted.HasValue || t.IsDeleted.Value == false)
                        TransactionsViewModels.Add(vm);
                    vm.PropertyChanged += OnCollectionItemPropertyChanged;
                }

                OnPropertyChanged(nameof(TransactionsViewModels));
                IsTransactionsLoaded = true;
                OnPropertyChanged(nameof(IsTransactionsLoaded));


            });
           
        }

        private async Task LoadPiggyBankInfoAsync()
        {
            await Task.Run(async () =>
            {
                var userdata = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, IsRemote);
                PiggyBank = await _piggyBankService.GetPiggyBankDetailsAsync(PiggyBank.Id, CancellationToken.None, IsRemote);
                PiggyBankCustomization = await _piggyBankService.GetPiggyBankCustomizationByPiggyBankIdAsync(PiggyBank.Id, CancellationToken.None, IsRemote);
                PiggyBankType = await _piggyBankService.GetPiggyBankTypeDetailsAsync(PiggyBankCustomization.PiggyBankTypeId, CancellationToken.None); // всегда local
                if (UserPiggyBank != null)
                {
                    try
                    {
                        UserPiggyBank = await _piggyBankService.GetUserPiggyBankLinkAsync(UserPiggyBank.Id, CancellationToken.None, IsRemote);
                    }
                    catch (NotFoundException ex)
                    {

                    }
                }
                OnPropertyChanged(nameof(PiggyBank));
                OnPropertyChanged(nameof(PiggyBankType));
                OnPropertyChanged(nameof(PiggyBankTypePhotoPath));
                OnPropertyChanged(nameof(PiggyBankCustomization));
                OnPropertyChanged(nameof(PhotoPath));
                OnPropertyChanged(nameof(UserPiggyBank));
                OnPropertyChanged(nameof(FillLevel));
                OnPropertyChanged(nameof(GoalBalance));
            });


        }

        private void OnCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var vm = sender as TransactionViewModel;
            if (e.PropertyName == nameof(TransactionViewModel.IsVisible))
                LoadDataAsync();
        }

            private async Task<UserDetailsDTO> GetUserDetailsAsync(int userId)
        {
            return await _userInfoService.GetUserDetailsAsync(userId, CancellationToken.None, _remote);
        }

        private async Task<ObservableCollection<TransactionDTO>> GetTransactionsAsync()
        {
            // реализовать логику
            var id = PiggyBank.Id;
            var result = await _transactionService.GetTransactionsByPiggyBankIdAsync(id, CancellationToken.None, _remote);
            return result.ToObservableCollection();
        }

        private async Task<ObservableCollection<PaymentTypeDTO>> GetPaymentTypesAsync()
        {
            var result = await _transactionService.GetAllPaymentTypesAsync(CancellationToken.None);
            return result.ToObservableCollection();
        }

        private async Task<ObservableCollection<TransactionTypeDTO>> GetTransactionTypesAsync()
        {
            var result = await _transactionService.GetAllTransactionTypesAsync(CancellationToken.None);
            return result.ToObservableCollection();
        }


        private async Task<bool> DisplayDeletePiggyBankPopupAsync(PiggyBankDTO piggyBank)
        {

            bool answer = await App.Current.MainPage.DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить копилку?", "Да", "Нет");
            if (answer)
            {
                if (!_remote)
                {
                    var updateDto = _mapper.Map<UpdatePiggyBankDTO>(piggyBank);
                    updateDto.IsDeleted = true;
                    await _piggyBankService.UpdatePiggyBankAsync(piggyBank.Id, updateDto, CancellationToken.None, false);
                }
                else
                    await _piggyBankService.DeletePiggyBankAsync(piggyBank.Id, CancellationToken.None, true);
                OnPropertyChanged(nameof(PiggyBank));
                OnPropertyChanged(nameof(IsPiggyBankDeleted));
            }
            return answer;
        }

        private async Task DisplayEditPrivacyPopupAsync()
        {
            if (UserPiggyBank != null)
            {
                await _popupService.ShowPopupAsync<PrivacyPopupViewModel>(async vm => { vm.LoadFromDTO((UserPiggyBankDTO)UserPiggyBank.Clone()); vm.ChangeDisplayMode(_remote); });
                LoadDataAsync();
            }
            else
                throw new NullReferenceException("UserPiggyBank is null"); 
            // тут не нужно делать 
        }


        private async Task DisplayCreateTransactionPopupAsync()
        {
            await _popupService.ShowPopupAsync<TransactionInfoPopupViewModel>(async vm => { await vm.LoadFromDTO(new TransactionDTO { PiggyBankId = PiggyBank.Id, Date = DateTime.UtcNow, PaymentTypeId = 0, TransactionTypeId = 0}, (await GetTransactionTypesAsync()).ToList(), (await GetPaymentTypesAsync()).ToList()); vm.ChangeDisplayMode(_remote); } );
            LoadDataAsync();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
