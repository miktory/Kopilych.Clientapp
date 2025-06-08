using AutoMapper;
using Bumptech.Glide.Load.Model;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Shared.DTO;
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
    public class FriendlistPopupViewModel : INotifyPropertyChanged
    {
        private Dictionary<RequestType, string> _requestTypesNames = new Dictionary<RequestType, string>();
        private string _selectedRequestTypeName;
        private IUserInfoService _userInfoService;
        private IPopupService _popupService;
        private INavigationService _navigationService;
        private IFileService _fileService;
        private IMapper _mapper;
        private BackButtonViewModel _backButtonViewModel;
        private ObservableCollection<UserInfoCardViewModel> _incomingRequests = new ObservableCollection<UserInfoCardViewModel>();
        private ObservableCollection<UserInfoCardViewModel> _approvedRequests = new ObservableCollection<UserInfoCardViewModel>();
        private ObservableCollection<UserInfoCardViewModel> _outgoingRequests = new ObservableCollection<UserInfoCardViewModel>();
        private ICommand _addFriendCommand;
        private ICommand _loadDataCommand;
        private bool _isRefreshing;

        public bool IsRefreshing { get => _isRefreshing; private set { _isRefreshing = value; OnPropertyChanged(nameof(IsRefreshing)); } }

        public ObservableCollection<string> RequestTypeNames { get => _requestTypesNames.Values.ToObservableCollection();  }
        public BackButtonViewModel BackButtonViewModel { get => _backButtonViewModel; private set { 
                _backButtonViewModel = value; OnPropertyChanged(nameof(BackButtonViewModel)); } }
        public string SelectedRequestTypeName { get => _selectedRequestTypeName; set { _selectedRequestTypeName = value; OnPropertyChanged(nameof(DisplayableRequests)); } }
        public ICommand AddFriendCommand { get => _addFriendCommand; private set { _addFriendCommand = value; OnPropertyChanged(nameof(AddFriendCommand)); } }
        public ICommand LoadDataCommand { get => _loadDataCommand; private set { _loadDataCommand = value; OnPropertyChanged(nameof(LoadDataCommand)); } }



        public ObservableCollection<UserInfoCardViewModel> DisplayableRequests
        {
            get
            {
                switch (SelectedRequestType)
                {
                    case RequestType.Approved:
                        return _approvedRequests;

                    case RequestType.Incoming:
                        return _incomingRequests;

                    case RequestType.Outgoing:
                        return _outgoingRequests;
                }
                return new ObservableCollection<UserInfoCardViewModel>();
            }
        }

        public RequestType SelectedRequestType 
        {
           get => _requestTypesNames.FirstOrDefault(x => x.Value == SelectedRequestTypeName).Key; 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public enum RequestType
        {
            Approved,
            Incoming,
            Outgoing,

        }


        public FriendlistPopupViewModel(BackButtonViewModel backButtonVm, IUserInfoService userInfoService, IPopupService popupService, IMapper mapper, INavigationService navigationService, IFileService fileService) 
        {
            _userInfoService = userInfoService;
            _popupService = popupService;
            _mapper = mapper;
            _navigationService = navigationService;
            BackButtonViewModel = backButtonVm;
            _fileService = fileService;
            Init();
        }

        public void Init()
        {
           _requestTypesNames = new Dictionary<RequestType, string>() { { RequestType.Approved, "Подтверждённые" }, { RequestType.Incoming, "Входящие" }, { RequestType.Outgoing, "Исходящие" } };
            SelectedRequestTypeName = _requestTypesNames.FirstOrDefault().Value;
            _incomingRequests = new ObservableCollection<UserInfoCardViewModel>();
            _outgoingRequests = new ObservableCollection<UserInfoCardViewModel>();
            _approvedRequests = new ObservableCollection<UserInfoCardViewModel>();
            AddFriendCommand = new Command(async () =>
            {
                string result = await App.Current.MainPage.DisplayPromptAsync("Добавить пользователя", "Введите ID пользователя:", "OK", "Отмена", "id", keyboard: Keyboard.Numeric);
                if (!string.IsNullOrEmpty(result))
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        try
                        {
                            var currUser = await _userInfoService.GetCurrentUserDetailsAsync(cts.Token, true);
                            _popupService.ShowPopupAsync<PreloaderPopupViewModel>(vm => vm.SetCancellationToken(cts.Token));
                            await _userInfoService.CreateFriendRequestAsync(new CreateFriendshipDTO { InitiatorUserId = currUser.ExternalId.Value, ApproverUserId = Int32.Parse(result) }, cts.Token, true);
                            await LoadDataAsync(cts.Token);
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
                    await LoadDataAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
                IsRefreshing = false;
            });

            UpdateAllPropertiesUI();
        }

        public async Task LoadDataAsync(CancellationToken cancellationToken)
        {

            await Task.Run(async () =>
            {
                foreach (var r in _approvedRequests.Concat(_outgoingRequests).Concat(_incomingRequests))
                {
                    try
                    {
                        r.PropertyChanged -= Item_PropertyChanged;
                    }
                    catch { }
                }
                _approvedRequests.Clear();
                _outgoingRequests.Clear();
                _incomingRequests.Clear();
                var currUser = await _userInfoService.GetCurrentUserDetailsAsync(cancellationToken, true);
                var friendRequests = await _userInfoService.GetAllUserFriendshipDetailsAsync(currUser.ExternalId.Value, cancellationToken, true);
                foreach (var request in friendRequests)
                {
                    var user = await _userInfoService.GetUserDetailsAsync(request.ApproverUserId == currUser.ExternalId.Value ? request.InitiatorUserId : request.ApproverUserId, cancellationToken, true);
                    UserInfoCardViewModel vm = null;
                    if (request.RequestApproved)
                    {
                        vm = new UserInfoCardViewModel(user, _popupService, _userInfoService, UserInfoCardViewModel.UserCardType.ApprovedFriendRequest, _mapper, _navigationService, _fileService, true, request);
                        vm.PropertyChanged += Item_PropertyChanged;
                        _approvedRequests.Add(vm);
                    }
                    else if (request.InitiatorUserId == currUser.ExternalId.Value)
                    {
                        vm = new UserInfoCardViewModel(user, _popupService, _userInfoService, UserInfoCardViewModel.UserCardType.OutgoingFriendRequest, _mapper, _navigationService, _fileService, true, request);
                        vm.PropertyChanged += Item_PropertyChanged;
                        _outgoingRequests.Add(vm);
                    }
                    else
                    {
                        vm = new UserInfoCardViewModel(user, _popupService, _userInfoService, UserInfoCardViewModel.UserCardType.IncomingFriendRequest, _mapper, _navigationService, _fileService, true, request);
                        vm.PropertyChanged += Item_PropertyChanged;
                        _incomingRequests.Add(vm);
                    }
                }
            });
          //  UpdateAllPropertiesUI();


        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == (nameof(UserInfoCardViewModel.FriendRequest)))
                LoadDataAsync(CancellationToken.None);
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
