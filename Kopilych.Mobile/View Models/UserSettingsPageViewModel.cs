using Android.Text;
using AutoMapper;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Kopilych.Application.Interfaces;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Mobile.Views;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kopilych.Mobile.View_Models
{
    public class UserSettingsPageViewModel : INotifyPropertyChanged
    {
        private IIntegrationService _integrationService;
        private IUserInfoService _userInfoService;
        private IMapper _mapper;
        private INavigationService _navigationService;
        private TaskCompletionSource<bool> _authTaskCompletionSource;
        private IFileService _fileService;
        public IPopupService _popupService;
        

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand OpenAuthPageCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand OpenFriendlistCommand { get; private set; }

        public bool IsSessionExists { get => _integrationService.IsSessionExists; }

        public UserInfoCardViewModel UserInfoCardViewModel { get; private set; }

        public UserSettingsPageViewModel(IIntegrationService integrationService, IPopupService popupService, IUserInfoService userInfoService, IMapper mapper, INavigationService navigationService, IFileService fileService)
        {
            _integrationService = integrationService;
            _popupService = popupService;
            _userInfoService = userInfoService;
            _integrationService.AuthCompleted += OnAuthCompleted;
            _mapper = mapper;
            _navigationService = navigationService;
            _fileService = fileService;
            OnPropertyChanged(nameof(IsSessionExists));
            Init();

        }


        public void OnAuthCompleted()
        {
            LoadDataAsync();
        }
        private void Init()
        {
            UserInfoCardViewModel = new UserInfoCardViewModel(new Shared.UserDetailsDTO() { ExternalId = null, Id = 0, Username = "...", Version = -1 }, _popupService, _userInfoService, UserInfoCardViewModel.UserCardType.OtherUserNoAction, _mapper, _navigationService, _fileService, false);
            OpenAuthPageCommand = new Command(async () => {
                try 
                {
                    var authResult = await WebAuthenticator.AuthenticateAsync(new Uri(_integrationService.LoginPageAddress), new Uri("kopilych://authcallback"));
                } catch (Exception ex) 
                {
                    CustomExceptionHandlerMiddleware.Handle(ex); 
                } 
            });

            LogoutCommand = new Command(async () => {
                try
                {
                    var confirm = await App.Current.MainPage.DisplayAlert("Подтверждение", "Вы действительно хотите отключиться от сервера? Локальные данные останутся сохранёнными.", "Да", "Нет");
                    if (confirm)
                    {
                        var session = await _userInfoService.GetCurrentUserSessionAsync(CancellationToken.None);
                        var isFull = await App.Current.MainPage.DisplayAlert("Выход", "Нужно ли завешить сеанс на остальных устройствах?", "Да", "Нет");
                        await _userInfoService.LogoutAsync(isFull, _mapper.Map<LogoutDTO>(session), CancellationToken.None);
                    }
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                     CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });

            OpenFriendlistCommand = new Command(async () => {
                using (var cts = new CancellationTokenSource())
                {
                    _popupService.ShowPopup<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cts.Token));
                    FriendlistPopupViewModel vm = null;
                    FriendlistPopupView view = null;
                    await Task.Run(async () =>
                    {
                        vm = Microsoft.Maui.MauiApplication.Current.Services.GetService<FriendlistPopupViewModel>();
                        view = new FriendlistPopupView(vm);
                        view.BindingContext = vm;

                    });
                    await _navigationService.NavigateToAsync((view));
                    cts.Cancel();
                }


            });



        }

        public async Task LoadDataAsync()
        {
            var currentUser = new UserDetailsDTO();
            var cts = new CancellationTokenSource();
            OnPropertyChanged(nameof(IsSessionExists));

            try
            {
                currentUser = await _userInfoService.GetCurrentUserDetailsAsync(cts.Token, false);
                UserInfoCardViewModel.LoadFromDTO(currentUser);
       
            }
            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);
            }
            OnPropertyChanged(nameof(UserInfoCardViewModel));
        }




        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
