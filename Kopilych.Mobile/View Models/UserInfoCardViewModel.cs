using Android.Media;
using AndroidX.Fragment.App.StrictMode;
using AutoMapper;
using CommunityToolkit.Maui.Core;
using Kopilych.Application.Common.Exceptions;
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kopilych.Mobile.View_Models
{
    public class UserInfoCardViewModel : INotifyPropertyChanged
    {
        private UserDetailsDTO _userDetails;
        private IPopupService _popupService;
        private IUserInfoService _userInfoService;
        private IMapper _mapper;
        private INavigationService _navigationService;
        private bool _remote;
        private IFileService _fileService;
        private ImageSource _userImageSource;
        private UserCardType _cardType;

        public UserFriendshipDetailsDTO FriendRequest { get; private set; }
        public ImageSource UserImageSource { get => _userImageSource; private set { _userImageSource = value; OnPropertyChanged(nameof(UserImageSource)); } }

        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get => _userDetails.Id; }
        public int? ExternalId { get => _userDetails.ExternalId; }
        public int Version { get => _userDetails.Version; }
        public string PhotoPath { get => string.IsNullOrEmpty(_userDetails.PhotoPath) ? DefaultPaths.UserImage : _userDetails.PhotoPath; }
        public string Username { get => _userDetails.Username; }
        public bool IsAcceptButtonVisible { get => CardType == UserCardType.IncomingFriendRequest; }
        public bool IsDeclineButtonVisible { get => CardType == UserCardType.OutgoingFriendRequest || CardType == UserCardType.ApprovedFriendRequest; }
        public UserCardType CardType { get => _cardType; private set { _cardType = value; OnPropertyChanged(nameof(CardType)); } }

        public enum UserCardType
        {
            CurrentUser,
            OtherUser,
            OtherUserNoAction,
            ApprovedFriendRequest,
            IncomingFriendRequest,
            OutgoingFriendRequest
        }

        public UserInfoCardViewModel(UserDetailsDTO userDetails, IPopupService popupService,IUserInfoService userInfoService, UserCardType userCardType, IMapper mapper, INavigationService navigationService, IFileService fileService, bool remote, UserFriendshipDetailsDTO friendRequest = null)
        {
            _userDetails = userDetails;
            _popupService = popupService;
            _userInfoService = userInfoService;
            FriendRequest = friendRequest;
            _mapper = mapper;
            CardType = userCardType;
            _navigationService = navigationService;
            _remote = remote;
            _fileService = fileService;
            Init();
        }

        private void Init()
        {
            UserImageSource = DefaultPaths.UserImage;
            CardClickCommand = new Command(async () => {
                if (CardType == UserCardType.CurrentUser)
                {
                    var result = await DisplayEditUserPopupAsync();
                    if (result != null)
                        LoadFromDTO(result);
                } else if (CardType != UserCardType.OtherUserNoAction ) 
                {
                    PiggyBanksGalleryPageViewModel piggyBanksGalleryPageVm = null;
                    PiggyBanksGalleryPageView piggyBanksGalleryPageView = null;
                    if (CardType != UserCardType.CurrentUser)
                    {
                        using (var cts = new CancellationTokenSource())
                        {
                            _popupService.ShowPopup<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cts.Token));

                            await Task.Run(async () =>
                            {
                                piggyBanksGalleryPageVm = Microsoft.Maui.MauiApplication.Current.Services.GetService<PiggyBanksGalleryPageViewModel>();
                                piggyBanksGalleryPageVm.ChangeDisplayMode(_userDetails.Id, true, false);
                                piggyBanksGalleryPageView = new PiggyBanksGalleryPageView(piggyBanksGalleryPageVm);
                                piggyBanksGalleryPageView.BindingContext = piggyBanksGalleryPageVm;


                            });
                            await _navigationService.NavigateToAsync((piggyBanksGalleryPageView));
                            cts.Cancel();
                        }


                    }
                }
            });

            OpenAvatarCommand = new Command(async () => { await DisplayImagePopupAsync(UserImageSource); });
            AcceptFriendRequestCommand = new Command(async () => {
                try
                {
                    if (FriendRequest != null)
                    {
                        var updateFriendshipDTO = _mapper.Map<UpdateFriendshipDTO>(FriendRequest);
                        updateFriendshipDTO.RequestApproved = true;
                        await _userInfoService.UpdateFriendRequestAsync(FriendRequest.Id, updateFriendshipDTO, CancellationToken.None, true);
                        FriendRequest.RequestApproved = true;
                        OnPropertyChanged(nameof(FriendRequest));
                    }
                }
                catch (Exception ex) 
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
            DeclineFriendRequestCommand = new Command(async () => {
                try
                {
                    if (FriendRequest != null)
                    {
                        await _userInfoService.DeleteFriendRequestAsync(FriendRequest.Id, CancellationToken.None, true);
                        FriendRequest = null;
                        OnPropertyChanged(nameof(FriendRequest));
                    }
                }
                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
            UpdateAllPropertiesUI();
        }

        public ICommand CardClickCommand { get; private set; }

        public ICommand OpenAvatarCommand { get; private set; }
        public ICommand AcceptFriendRequestCommand { get; private set; }
        public ICommand DeclineFriendRequestCommand { get; private set; }

        private async Task<UserDetailsDTO> DisplayEditUserPopupAsync()
        {
            var result = (UserDetailsDTO)(await _popupService.ShowPopupAsync<CreateOrEditUserPopupViewModel>(vm => vm.LoadFromDTO((UserDetailsDTO)_userDetails.Clone())));
            return result;
        }

        private async Task DisplayImagePopupAsync(ImageSource imageSource)
        {
            await _popupService.ShowPopupAsync<ImagePopupViewModel>(vm => vm.LoadImage(imageSource));
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

        public async Task LoadDataAsync()
        {
          
                await LoadImage(_remote);
           
        }

        private async Task LoadImage(bool remote)
        {
            try
            {
            await Task.Run(async () =>
            {
                if (!remote)
                {
                    if (_fileService.Exist(PhotoPath))
                        UserImageSource = ImageSource.FromFile(PhotoPath);
                    else
                    {
                        UserImageSource = ImageSource.FromFile(DefaultPaths.UserImage);
                    }
                }
                else
                {
                    byte[] image = null;

                    image = await _userInfoService.GetUserPhotoAsync(_userDetails, CancellationToken.None, true);
                    UserImageSource = ImageSource.FromStream(() => new MemoryStream(image));


                }
            });
            }
            catch (Exception ex)
            {
                if (ex is NotFoundException)
                    await Task.Run(() =>
                    {
                        UserImageSource = ImageSource.FromFile(DefaultPaths.UserImage);
                    });
                else
                    CustomExceptionHandlerMiddleware.Handle(ex);
            }
        }

        public void ChangeDisplayMode(bool remote, UserCardType userCardType)
        {
            _remote = remote;
            CardType = userCardType;
        }

        public void LoadFromDTO(UserDetailsDTO userDetails, UserFriendshipDetailsDTO userFriendshipDetailsDTO = null)
        {
            _userDetails = userDetails;
            FriendRequest = userFriendshipDetailsDTO;
            LoadDataAsync();
            UpdateAllPropertiesUI();
          
        }
    }
}
