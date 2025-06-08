using Android.App;
using AutoMapper;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kopilych.Mobile.View_Models
{
    public class PiggyBankMembersPageViewModel : INotifyPropertyChanged
    {
        private IUserInfoService _userInfoService;
        private IPopupService _popupService;
        private PiggyBankDTO _piggyBank;
        private UserDetailsDTO _currentUser;
        private IMapper _mapper;
        private INavigationService _navigationService;
        private IFileService _fileService;
        private IPiggyBankService _piggyBankService;

        private BackButtonViewModel _backButtonViewModel;
        public ObservableCollection<KeyValuePair<UserInfoCardViewModel, MemberAction>> _membersActionCollection = new ObservableCollection<KeyValuePair<UserInfoCardViewModel, MemberAction>>();
        private ICommand _inviteUserCommand;
        private bool _isRefreshing;
        private ICommand _loadDataCommand;
        private bool _isLoaded;

        public event PropertyChangedEventHandler? PropertyChanged;
        public UserDetailsDTO CurrentUser { get => _currentUser; private set { _currentUser = value; OnPropertyChanged(nameof(CurrentUser)); } }
        public PiggyBankDTO PiggyBank { get => _piggyBank; private set { _piggyBank = value; OnPropertyChanged(nameof(PiggyBank)); } }
        public ICommand InviteUserCommand { get => _inviteUserCommand; private set { _inviteUserCommand = value; OnPropertyChanged(nameof(InviteUserCommand)); } }
        public ICommand LoadDataCommand { get => _loadDataCommand; private set { _loadDataCommand = value; OnPropertyChanged(nameof(LoadDataCommand)); } }
        public bool IsRefreshing { get => _isRefreshing; private set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }
        public bool IsLoaded { get => _isLoaded; private set { _isLoaded = value; OnPropertyChanged(nameof(IsLoaded)); } }
        public BackButtonViewModel BackButtonViewModel
        {
            get => _backButtonViewModel; private set
            {
                _backButtonViewModel = value; OnPropertyChanged(nameof(BackButtonViewModel));
            }
        }

        public ObservableCollection<KeyValuePair<UserInfoCardViewModel, MemberAction>> MembersActionCollection { get => _membersActionCollection; private set { _membersActionCollection = value; OnPropertyChanged(nameof(MembersActionCollection)); } }
        public PiggyBankMembersPageViewModel(PiggyBankDTO piggyBank, UserDetailsDTO currentUser, BackButtonViewModel backButtonVm, IUserInfoService userInfoService, IPopupService popupService, IMapper mapper, INavigationService navigationService, IPiggyBankService piggyBankService, IFileService fileService)
        {
            _userInfoService = userInfoService;
            _popupService = popupService;
            PiggyBank = piggyBank;
            CurrentUser = currentUser;
            _mapper = mapper;
            _navigationService = navigationService;
            _piggyBankService = piggyBankService;
            BackButtonViewModel = backButtonVm;
            _fileService = fileService;
            Init();
        }

        public void Init()
        {

            MembersActionCollection = new ObservableCollection<KeyValuePair<UserInfoCardViewModel, MemberAction>>();
            InviteUserCommand = new Command(async () =>
            {
                string result = await App.Current.MainPage.DisplayPromptAsync("Добавить пользователя", "Введите ID пользователя:", "OK", "Отмена", "id", keyboard:Keyboard.Numeric);
                if (!string.IsNullOrEmpty(result))
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        try
                        {
                            _popupService.ShowPopupAsync<PreloaderPopupViewModel>(vm => vm.SetCancellationToken(cts.Token));
                            await _piggyBankService.CreateUserPiggyBankLinkAsync(new CreateUserPiggyBankDTO { PiggyBankId = PiggyBank.ExternalId.Value, UserId = Int32.Parse(result)}, cts.Token, true);
                            LoadMembersAsync(CancellationToken.None);
                        }
                        catch (Exception ex)
                        {
                            CustomExceptionHandlerMiddleware.Handle(ex);
                        }
                        cts.Cancel();
                    }
                }
            });

            LoadDataCommand = new Command(async () =>
            {
                    IsRefreshing = true;
                    try
                    {
                        await LoadMembersAsync(CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        CustomExceptionHandlerMiddleware.Handle(ex);
                    }
                    IsRefreshing = false;
            });

        }

        public void LoadFromDTO(PiggyBankDTO piggyBank, UserDetailsDTO currentUser)
        {
            PiggyBank = piggyBank;
            CurrentUser = currentUser;
        }

        public PiggyBankMembersPageViewModel()
        {
         
        }

        public class MemberAction
        {
            public bool IsEnabled { get; set; }
            public ICommand Command { get; set; }
        }

        public async Task LoadMembersAsync(CancellationToken cancellationToken)
        {
            IsLoaded = false;
                MembersActionCollection.Clear();
            var result = new List<KeyValuePair<UserInfoCardViewModel, MemberAction>>();
            await Task.Run(async () =>
            {
                var members = await _piggyBankService.GetUserPiggyBankLinksByPiggyBankIdAsync(PiggyBank.ExternalId.Value, cancellationToken, true);


                var userTasks = members.Select(m =>
      _userInfoService.GetUserDetailsAsync(m.UserId, cancellationToken, true)
  ).ToList();
                var users = await Task.WhenAll(userTasks);


                foreach (var m in members)
                {
                    UserInfoCardViewModel userinfovm = null;
                    // var user = await _userInfoService.GetUserDetailsAsync(m.UserId, cancellationToken, true);
                    var user = users.First(u => u.Id == m.UserId);
                    userinfovm = new UserInfoCardViewModel(user, _popupService, _userInfoService, UserInfoCardViewModel.UserCardType.OtherUser, _mapper, _navigationService, _fileService, true);
                    var command = new Command(async () =>
                    {
                        var confirm = await App.Current.MainPage.DisplayAlert("Подтверждение", "Удалить участника копилки?", "Да", "Нет");
                        if (confirm)
                        {

                            using (var cts = new CancellationTokenSource())
                            {
                                try
                                {
                                    _popupService.ShowPopupAsync<PreloaderPopupViewModel>(vm => vm.SetCancellationToken(cts.Token));
                                    await _piggyBankService.DeleteUserPiggyBankAsync(m.ExternalId.Value, cts.Token, true);
                                    await LoadMembersAsync(cts.Token);
                                }
                                catch (Exception ex)
                                {
                                    CustomExceptionHandlerMiddleware.Handle(ex);
                                }
                                cts.Cancel();
                            }
                        }
                    });

                    var enabled = true;
                    if (CurrentUser.ExternalId.Value != PiggyBank.OwnerId && m.UserId != CurrentUser.ExternalId.Value)
                        enabled = false;

                    result.Add(new KeyValuePair<UserInfoCardViewModel, MemberAction>(userinfovm, new MemberAction { Command = command, IsEnabled = enabled }));
                }
            });
                foreach (var m in result)
                    MembersActionCollection.Add(m);
            IsLoaded = true;

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
