
using Android.Content;
using Android.Media;
using Android.OS;
using AutoMapper;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.CreateUser;
using Kopilych.Application.CQRS.Queries.User.GetCurrentUserDetails;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByUserId;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Mobile.Views;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Collections.Specialized.BitVector32;

namespace Kopilych.Mobile.View_Models
{
    public class PiggyBanksGalleryPageViewModel : INotifyPropertyChanged
    {
        private bool _isPersonalPiggyBanksVisible;
        private bool _isGroupPiggyBanksVisible;
        private IUserInfoService _userInfoService;
        private IPiggyBankService _piggyBankService;
        private IPopupService _popupService;
        private IMapper _mapper;
        private int _userId;
        private bool _remote;
        private bool _isCurrentUser;
        private IIntegrationService _integrationService;
        private Task _piggyBankIntegrationTask;
        private Task _userIntegrationTask;
        private INavigationService _navigationService;
        private ITransactionService _transactionService;
        private IFileService _fileService;
        private UserDetailsDTO _user;
        private BackButtonViewModel _backButtonViewModel;


        public BackButtonViewModel BackButtonViewModel { get  { return _backButtonViewModel; } set { _backButtonViewModel = value; OnPropertyChanged(nameof(BackButtonViewModel)); } }
        public ObservableCollection<PiggyBankCardViewModel> PersonalPiggyBankCardsViewModels { get; private set; }
        public ObservableCollection<PiggyBankCardViewModel> SharedPiggyBankCardsViewModels { get; private set; }

        public UserInfoCardViewModel UserInfoCardViewModel { get; private set; }
        public bool IsPersonalPiggyBanksVisible
        {
            get { return _isPersonalPiggyBanksVisible; }
            set
            {
                if (_isPersonalPiggyBanksVisible != value)
                {
                    _isPersonalPiggyBanksVisible = value;
                    OnPropertyChanged(nameof(IsPersonalPiggyBanksVisible));
                }
            }
        }

        public bool IsCurrentUser
        {
            get { return _isCurrentUser; }
            set
            {
                if (_isCurrentUser != value)
                {
                    _isCurrentUser = value;
                    OnPropertyChanged(nameof(IsCurrentUser));
                }
            }
        }

        public bool IsGroupPiggyBanksVisible
        {
            get { return _isGroupPiggyBanksVisible; }
            set
            {
                if (_isGroupPiggyBanksVisible != value)
                {
                    _isGroupPiggyBanksVisible = value;
                    OnPropertyChanged(nameof(IsGroupPiggyBanksVisible));

                }
            }
        }

        public bool IsRefreshing { get; private set; }
        public ICommand TogglePersonalPiggyBanksVisibilityCommand { get; private set; }
        public ICommand ToggleGroupPiggyBanksVisibilityCommand { get; private set; }
        public ICommand CreatePiggyBankCommand { get; private set; }
        public ICommand RefreshDataCommand { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private IMediator _mediator;


        public PiggyBanksGalleryPageViewModel(BackButtonViewModel backButtonVm, IMediator mediator, IUserInfoService userInfoService, IPopupService popupService, IPiggyBankService piggyBankService, IMapper mapper, IIntegrationService integrationService, INavigationService navigationService, ITransactionService transactionService, IFileService fileService, int userId, bool remote, bool isCurrentUser)
        {
            _mapper = mapper;
            _piggyBankService = piggyBankService;
            _popupService = popupService;
            _mediator = mediator;
            _userInfoService = userInfoService;
            _userId = userId;
            _remote = remote;
            _isCurrentUser = isCurrentUser;
            _integrationService = integrationService;
            _navigationService = navigationService;
            _transactionService = transactionService;
            BackButtonViewModel = backButtonVm;
            _fileService = fileService;
            Init();

            //PiggyBankCardsViewModels.Add(new PiggyBankCardViewModel(piggyBank1, piggybankCustom1));
        }

        public void Init()
        {
            TogglePersonalPiggyBanksVisibilityCommand = new Command(() => { IsPersonalPiggyBanksVisible = !IsPersonalPiggyBanksVisible; });
            ToggleGroupPiggyBanksVisibilityCommand = new Command(() => { IsGroupPiggyBanksVisible = !IsGroupPiggyBanksVisible; });
            CreatePiggyBankCommand = new Command(async () => {
                if (_isCurrentUser)
                await DisplayCreatePiggyBankPopup(); });
            PersonalPiggyBankCardsViewModels = new ObservableCollection<PiggyBankCardViewModel>();
            SharedPiggyBankCardsViewModels = new ObservableCollection<PiggyBankCardViewModel>();
            UserInfoCardViewModel = new UserInfoCardViewModel(new Shared.UserDetailsDTO() { ExternalId = null, Id = 0, Username = "...", Version = -1 }, _popupService,_userInfoService,
                _isCurrentUser ? UserInfoCardViewModel.UserCardType.CurrentUser : UserInfoCardViewModel.UserCardType.OtherUserNoAction, _mapper, _navigationService, _fileService, _remote);
            RefreshDataCommand = new Command(async () => { IsRefreshing = true; LoadDataAsync(CancellationToken.None); IsRefreshing = false; OnPropertyChanged(nameof(IsRefreshing)); });

            if (!_integrationService.IsConfigured)
            {
                try
                {
                    _integrationService.Configure();
                }
                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            }

            UpdateAllPropertiesUI();
        }
        public PiggyBanksGalleryPageViewModel() { }

        public async Task StartNewbieHelperAsync(CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userInfoService.GetCurrentUserDetailsAsync(cancellationToken, false);
            }
            catch (NotFoundException ex)
            {
                // Create User
                //await _mediator.Send(new CreateUserCommand { ExternalId = null, Username = "Пользователь", InitiatorUserId = 0, IsExecuteByAdmin = true });
                await DisplayCreateUserPopup();
            }
        }

        public void DisplayPreloader(CancellationToken cancellationToken)
        {
            Device.BeginInvokeOnMainThread(() => _popupService.ShowPopupAsync<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cancellationToken)));
        }

        public async Task LoadDataAsync(CancellationToken cancellationToken)
        {

            _user = new UserDetailsDTO();
            var cts = new CancellationTokenSource();
            var integrationStatus = false;
            DisplayPreloader(cts.Token);
            using (var checkerCts = new CancellationTokenSource(5000))
            {
             integrationStatus = await _integrationService.CheckIfServerOnline(checkerCts.Token);
            }
            await Task.Run(async () =>
            {
                try
                {
                    PersonalPiggyBankCardsViewModels.Clear();
                    SharedPiggyBankCardsViewModels.Clear();
                    if (_isCurrentUser)
                    {
                        _user = await _userInfoService.GetCurrentUserDetailsAsync(cancellationToken, false);
                        if (_integrationService.IsSessionExists && integrationStatus &&  _user.ExternalId != null)
                        {
                            try
                            {
                                // тут интеграция всей инфы по юзеру
                                if (_userIntegrationTask == null || _userIntegrationTask.IsCompleted)
                                {
                                    _userIntegrationTask = RunUserIntegration();
                                    await _userIntegrationTask;
                                }

                                // тут специально не ждём, чтобы выполнялось в фоне 
                            }
                            catch (Exception ex)
                            {
                                CustomExceptionHandlerMiddleware.Handle(ex); //ошибка интеграции не должна мешать загрузке других данных
                            }
                            try
                            {
                                // тут интеграция всей инфы по копилочкам
                                if (_piggyBankIntegrationTask == null || _piggyBankIntegrationTask.IsCompleted)
                                {
                                    _piggyBankIntegrationTask = RunPiggyBankIntegration();
                                    await _piggyBankIntegrationTask;
                                }

                            }
                            catch (Exception ex)
                            {
                                CustomExceptionHandlerMiddleware.Handle(ex); //ошибка интеграции не должна мешать загрузке других данных
                            }
                        }
                    }
                    else
                    {
                        _user = await _userInfoService.GetUserDetailsAsync(_userId, cancellationToken, _remote);
                    }

                    UserInfoCardViewModel.ChangeDisplayMode(_remote, _isCurrentUser ? UserInfoCardViewModel.UserCardType.CurrentUser : UserInfoCardViewModel.UserCardType.OtherUserNoAction);
                    UserInfoCardViewModel.LoadFromDTO(_user);

                    List<UserPiggyBankDTO> piggyBankLinks = new List<UserPiggyBankDTO>();
                    foreach (var p in PersonalPiggyBankCardsViewModels)
                    {
                        try
                        {
                            p.PropertyChanged -= On_PiggyBankCardPropertyChanged;
                        }
                        catch { }
                    }
                    foreach (var g in SharedPiggyBankCardsViewModels)
                    {
                        try
                        {
                            g.PropertyChanged -= On_PiggyBankCardPropertyChanged;
                        }
                        catch { }
                    }
                    try
                    {
                        PersonalPiggyBankCardsViewModels = new ObservableCollection<PiggyBankCardViewModel>((await PreparePiggyBankCardViewModels(_user, _remote, cancellationToken)).Where(x => !x.PiggyBank.Shared));
                        if (_isCurrentUser && _integrationService.IsSessionExists && _user.ExternalId != null && integrationStatus)
                            SharedPiggyBankCardsViewModels = new ObservableCollection<PiggyBankCardViewModel>((await PreparePiggyBankCardViewModels(_user, true, cancellationToken)).Where(x => x.PiggyBank.Shared));
                        foreach (var p in PersonalPiggyBankCardsViewModels)
                            p.PropertyChanged += On_PiggyBankCardPropertyChanged;

                        foreach (var g in SharedPiggyBankCardsViewModels)
                            g.PropertyChanged += On_PiggyBankCardPropertyChanged;

                    }
                    catch (Exception ex)
                    {
                        CustomExceptionHandlerMiddleware.Handle(ex);
                    }


                    UpdateAllPropertiesUI();
                }
                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
            cts.Cancel();
            cts.Dispose();

        }

        private void On_PiggyBankCardPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PiggyBankCardViewModel.IsVisible))
                LoadDataAsync(CancellationToken.None);
        }

        private async Task<List<PiggyBankCardViewModel>> PreparePiggyBankCardViewModels(UserDetailsDTO user, bool remote, CancellationToken cancellationToken)
        {
            var links = new List<UserPiggyBankDTO>();
            var result = new List<PiggyBankCardViewModel>();
            var currentUserLinks = new List<UserPiggyBankDTO>();
            links.AddRange(await _piggyBankService.GetUserPiggyBankLinksByUserIdAsync(remote ? user.ExternalId.Value : user.Id, cancellationToken, remote));
            if (_isCurrentUser)
            { 
                var currentUser = await _userInfoService.GetCurrentUserDetailsAsync(cancellationToken, remote);
                currentUserLinks.AddRange(await _piggyBankService.GetUserPiggyBankLinksByUserIdAsync(currentUser.Id, cancellationToken, remote));
            }
            foreach (var link in links)
            {
                // т.к. некоторые front-side ограничения определяются по UserPiggyBank - нам нужно передать экземпляр в карточку, а карточка передаст в страницу. 
                // передавать надо экземпляр, связанный именно с текущим пользователем (либо null, если его нет), если передадим экземпляр пользователя, по которому открыта PiggyBankGalleryPage - фронт будет думать, что мы и есть этот пользователь
                var piggyBank = await _piggyBankService.GetPiggyBankDetailsAsync(link.PiggyBankId, CancellationToken.None, remote);
                var customization = await _piggyBankService.GetPiggyBankCustomizationByPiggyBankIdAsync(link.PiggyBankId, cancellationToken, remote);
                if (!piggyBank.IsDeleted.HasValue || piggyBank.IsDeleted.Value == false)
                    result.Add(new PiggyBankCardViewModel(_isCurrentUser ? link: currentUserLinks.FirstOrDefault(x => x.PiggyBankId == piggyBank.Id), piggyBank, customization, _piggyBankService, _mapper, _navigationService, _fileService, _popupService, _isCurrentUser && (remote ? user.ExternalId.HasValue && user.ExternalId.Value == piggyBank.OwnerId : user.Id == piggyBank.OwnerId), remote));
            }
            return result;
        }

        public void ChangeDisplayMode(int userId, bool remote, bool isCurrentUser)
        {
            _userId = userId;
            _remote = remote;
            _isCurrentUser = isCurrentUser;
            IsCurrentUser = isCurrentUser;
        }


        private async Task RunUserIntegration() 
        {
           var localUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false);
           var externalUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, true);
            var result =  await _userInfoService.RunTwoWayUserIntegration(localUser, externalUser, CancellationToken.None);
            UserInfoCardViewModel.LoadFromDTO(result);
        }

        private async Task RunPiggyBankIntegration() 
        {
            var localUser = await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false);
            var localLinks = await _piggyBankService.GetUserPiggyBankLinksByUserIdAsync(localUser.Id, CancellationToken.None, false);
            var externalLinks = await _piggyBankService.GetUserPiggyBankLinksByUserIdAsync(localUser.ExternalId.Value, CancellationToken.None, true);
            List<PiggyBankDTO> localPiggyBanks = new List<PiggyBankDTO>();
            List<PiggyBankCustomizationDTO> localPiggyBankCustomizations = new List<PiggyBankCustomizationDTO>();
            foreach (var link in localLinks)
            {
                localPiggyBanks.Add(await _piggyBankService.GetPiggyBankDetailsAsync(link.PiggyBankId, CancellationToken.None, false));
                localPiggyBankCustomizations.Add(await _piggyBankService.GetPiggyBankCustomizationByPiggyBankIdAsync(link.PiggyBankId, CancellationToken.None, false));
            }
            // проверяем данные по external копилкам
            foreach (var link in externalLinks)
            {
                try
                {
                    PiggyBankDTO piggyBank = null;
                    try
                    {
                        piggyBank = await _piggyBankService.GetPiggyBankDetailsAsync(link.PiggyBankId, CancellationToken.None, true);
                    }
                    catch (NotFoundException) { }
                    if (piggyBank == null || localUser.ExternalId != piggyBank.OwnerId || piggyBank.Shared)
                        continue;
                    var localPiggyBank = localPiggyBanks.Where(x => x.ExternalId == piggyBank.Id).FirstOrDefault();
                    if (localPiggyBank != null && localPiggyBank.IsDeleted.HasValue && localPiggyBank.IsDeleted.Value == true)
                        continue;
                    var localUserPiggyBank = localLinks.Where(x => x.ExternalId == link.Id).FirstOrDefault();

                    await _piggyBankService.RunTwoWayPiggyBankIntegration(localPiggyBank, piggyBank, CancellationToken.None);

                    await _piggyBankService.RunTwoWayUserPiggyBankIntegration(localUserPiggyBank, link, CancellationToken.None);
                    PiggyBankCustomizationDTO customization = null;
                    try
                    {
                        customization = await _piggyBankService.GetPiggyBankCustomizationByPiggyBankIdAsync(link.PiggyBankId, CancellationToken.None, true);
                    }
                    catch (NotFoundException ex) { }
                    if (customization != null)
                    {
                        PiggyBankCustomizationDTO localPiggyBankCustomization = localPiggyBankCustomizations.Where(x => x.ExternalId == customization.Id).FirstOrDefault();
                            await _piggyBankService.RunTwoWayPiggyBankCustomizationIntegration(localPiggyBankCustomization, customization, CancellationToken.None);
                    }
                    List<TransactionDTO> localTransactions = new List<TransactionDTO>();
                    List<TransactionDTO> externalTransactions = new List<TransactionDTO>();
                    if (localPiggyBank != null)
                        localTransactions = await _transactionService.GetTransactionsByPiggyBankIdAsync(localPiggyBank.Id, CancellationToken.None, false);
                    externalTransactions = await _transactionService.GetTransactionsByPiggyBankIdAsync(piggyBank.Id, CancellationToken.None, true);
                    foreach (var t in externalTransactions)
                    {
                        var localTransaction = localTransactions.Where(x => x.ExternalId == t.Id).FirstOrDefault();
                        await _transactionService.RunTwoWayTransactionIntegration(localTransaction, t, CancellationToken.None);
                    }
                }
                catch (Exception ex) 
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            }


            // проверяем данные по локальным копилкам
            localLinks = await _piggyBankService.GetUserPiggyBankLinksByUserIdAsync(localUser.Id, CancellationToken.None, false);
            externalLinks = await _piggyBankService.GetUserPiggyBankLinksByUserIdAsync(localUser.ExternalId.Value, CancellationToken.None, true);
            localPiggyBanks.Clear();
            localPiggyBankCustomizations.Clear();
            foreach (var link in localLinks)
            {
                localPiggyBanks.Add(await _piggyBankService.GetPiggyBankDetailsAsync(link.PiggyBankId, CancellationToken.None, false));
                localPiggyBankCustomizations.Add(await _piggyBankService.GetPiggyBankCustomizationByPiggyBankIdAsync(link.PiggyBankId, CancellationToken.None, false));
            }
            foreach (var link in localLinks)
            {
                try
                {
                    var localPiggyBank = localPiggyBanks.Where(x => x.Id == link.PiggyBankId).First();
                    PiggyBankDTO externalPiggyBank = null;
                    if (localPiggyBank.ExternalId == null)
                    {
                        var ans = (await _piggyBankService.RunTwoWayPiggyBankIntegration(localPiggyBank, null, CancellationToken.None));
                        if (ans != null)
                            localPiggyBank.ExternalId = ans.ExternalId;
                    }
                    else
                    {
                        try
                        {
                            externalPiggyBank = await _piggyBankService.GetPiggyBankDetailsAsync(localPiggyBank.ExternalId.Value, CancellationToken.None, true);
                        }
                        catch (NotFoundException ex) { }
                        await _piggyBankService.RunTwoWayPiggyBankIntegration(localPiggyBank, externalPiggyBank, CancellationToken.None);
                    }
                    if (localPiggyBank.IsDeleted.HasValue && localPiggyBank.IsDeleted.Value == true)
                        continue;
                    if (link.ExternalId != null)
                        await _piggyBankService.RunTwoWayUserPiggyBankIntegration(link, externalLinks.Where(x => x.Id == link.ExternalId.Value).FirstOrDefault(), CancellationToken.None);
                    else
                        await _piggyBankService.RunTwoWayUserPiggyBankIntegration(link, null, CancellationToken.None);
                    PiggyBankCustomizationDTO externalCustomization = null;
                    PiggyBankCustomizationDTO localCustomization = null;
                    try
                    {
                        externalCustomization = await _piggyBankService.GetPiggyBankCustomizationByPiggyBankIdAsync(localPiggyBank.ExternalId.Value, CancellationToken.None, true);
                    }
                    catch (NotFoundException ex) { }
                    localCustomization = localPiggyBankCustomizations.Where(x => x.PiggyBankId == localPiggyBank.Id).FirstOrDefault();
                    await _piggyBankService.RunTwoWayPiggyBankCustomizationIntegration(localCustomization, externalCustomization, CancellationToken.None);

                    List<TransactionDTO> localTransactions = new List<TransactionDTO>();
                    List<TransactionDTO> externalTransactions = new List<TransactionDTO>();
                    if (localPiggyBank != null)
                        localTransactions = await _transactionService.GetTransactionsByPiggyBankIdAsync(localPiggyBank.Id, CancellationToken.None, false);
                    externalTransactions = await _transactionService.GetTransactionsByPiggyBankIdAsync(localPiggyBank.ExternalId.Value, CancellationToken.None, true);
                    foreach (var t in localTransactions)
                    {
                        var externalTransaction = externalTransactions.Where(x => x.Id == t.ExternalId).FirstOrDefault();
                        await _transactionService.RunTwoWayTransactionIntegration(t, externalTransaction, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            }


        }

        

        private async Task DisplayCreateUserPopup()
        {
            await _popupService.ShowPopupAsync<CreateOrEditUserPopupViewModel>(vm => vm.LoadFromDTO(null));
        }

        private async Task DisplayCreatePiggyBankPopup()
        {
            await _popupService.ShowPopupAsync<PiggyBankInfoPopupViewModel>(vm => vm.ChangeDisplayMode(true, true));
            await LoadDataAsync(CancellationToken.None);

        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    }
}
