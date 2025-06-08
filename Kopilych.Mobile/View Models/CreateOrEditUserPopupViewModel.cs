using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using MediatR;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;


namespace Kopilych.Mobile.View_Models
{
    public class CreateOrEditUserPopupViewModel: INotifyPropertyChanged
    {
        private UserDetailsDTO _userDetails;
        private IUserInfoService _userInfoService;
        private IFileService _fileService;
        private IPopupService _popupService;
        private ImageSource _userImageSource;
        private byte[] _newImage;

        public event PropertyChangedEventHandler PropertyChanged;
        public int Id { get => _userDetails.Id; }
        public int? ExternalId { get => _userDetails.ExternalId; }
        public string PhotoPath { get => _userDetails.PhotoPath;  }
        public int Version { get => _userDetails.Version; }
        public string Username { get => _userDetails.Username; set { _userDetails.Username = value; } }

        public ImageSource UserImageSource { get => _userImageSource; private set { _userImageSource = value; OnPropertyChanged(nameof(UserImageSource)); } }
        public string UserPhotoPath { get => _userDetails.PhotoPath; private set { } }

        public bool IsUserExists { get; private set; }

        public ICommand ChangePhotoCommand { get; private set; }

        public ICommand CreateOrEditUserCommand { get; private set; }



        public CreateOrEditUserPopupViewModel(IUserInfoService userInfoService, IPopupService popupService, IFileService fileService) 
        {
            _userDetails = new UserDetailsDTO() { Username = "" };
            _popupService = popupService;
            _userInfoService = userInfoService;
            _fileService = fileService;
            Init();
        } 

        public CreateOrEditUserPopupViewModel(UserDetailsDTO? userDetails, IUserInfoService userInfoService, IPopupService popupService, IFileService fileService)
        {
            _userDetails = userDetails;
            _popupService = popupService;
            _userInfoService = userInfoService;
            _fileService = fileService;
            IsUserExists = true;
            Init();
        }
        
        public async void LoadFromDTO(UserDetailsDTO? userDetails)
        {
            _userDetails = userDetails;
            IsUserExists = true;
            if (_userDetails == null)
            {
                IsUserExists = false;
                _userDetails = new UserDetailsDTO() { Username = "" };
            }
            LoadDataAsync();
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Проходим по каждому свойству и вызываем OnPropertyChanged
            foreach (var property in properties)
            {
                OnPropertyChanged(property.Name);
            }
        }
        public async Task LoadDataAsync()
        {
            await LoadImage(false);
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

                    image = await _userInfoService.GetUserPhotoAsync(_userDetails, CancellationToken.None, true);
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


        private void Init()
        {

            ChangePhotoCommand = new Command(async () => {
                await LoadNewPhotoAsync(); });
            CreateOrEditUserCommand = new Command(async () => 
            {
                using (var cts = new CancellationTokenSource(30000))
                {
                    try
                    {
                        if (!IsUserExists)
                        {
                            await _userInfoService.CreateUserAsync(Username, PhotoPath, null, cts.Token);
                            _userDetails = await _userInfoService.GetCurrentUserDetailsAsync(cts.Token, false);
                            IsUserExists = true;
                            OnPropertyChanged(nameof(IsUserExists));
                            if (_newImage != null)
                                _userDetails.PhotoPath = await _userInfoService.UploadUserPhotoAsync(_userDetails.Id, _newImage, cts.Token, false);
                        }
                        else
                        {
                            var updateUserDTO = new UpdateUserDTO { ExternalId = ExternalId, PhotoPath = PhotoPath, Username = Username, Version = Version + 1 };
                            await _userInfoService.UpdateUserAsync(Id, updateUserDTO, cts.Token, false);
                            _userDetails = await _userInfoService.GetCurrentUserDetailsAsync(cts.Token, false);
                            IsUserExists = true;
                            OnPropertyChanged(nameof(IsUserExists));
                            if (_newImage != null)
                                _userDetails.PhotoPath = await _userInfoService.UploadUserPhotoAsync(_userDetails.Id, _newImage, cts.Token, false);
                        }
                        await OnSave();
                    }
                    catch (Exception ex) 
                    {
                        CustomExceptionHandlerMiddleware.Handle(ex);
                    }


                }

            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void ChangeUsernameAsync(string username)
        {
            _userDetails.Username = username;
            OnPropertyChanged(nameof(username));
        }



        public async Task LoadNewPhotoAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите фото"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    var fileSize = stream.Length / 1024 / 1024; // В мегабайтах
                    if (fileSize > 1)
                    {
                        await App.Current.MainPage.DisplayAlert("Ошибка", "Файл должен быть менее 1 МБ.", "OK");
                        return;
                    }

                    // Загружаем изображение с помощью SkiaSharp
                    using var originalImage = SKBitmap.Decode(stream);

                    SKImage image = SKImage.FromBitmap(originalImage);
                    SKData jpegData = image.Encode(SKEncodedImageFormat.Jpeg, 100); // 100 - максимальное качество

                    _newImage = jpegData.ToArray();

                    UserImageSource = ImageSource.FromStream(() =>
                    {
                        return new MemoryStream(_newImage);
                    });
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось загрузить фото.", "OK");
            }
        }



        async Task OnSave()
        {
           await _popupService.ClosePopupAsync(_userDetails);
        }
    }
}
