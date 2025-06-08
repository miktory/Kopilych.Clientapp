using Android.Media;
using AutoMapper;
using CommunityToolkit.Maui.Core;
using Java.Time;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Mobile.Views;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.View_Models 
{
    public class PiggyBankCardViewModel : INotifyPropertyChanged
    {
        private IPiggyBankService _piggyBankService;
        private INavigationService _navigationService;
        private IFileService _fileService;
        private IMapper _mapper;
        private IPopupService _popupService;
        private bool _isEditable;
        private bool _remote;
        private ImageSource _piggyBankImageSource;

        public PiggyBankDTO PiggyBank { get; private set; } = new PiggyBankDTO();

        public string PhotoPath { get => string.IsNullOrEmpty(PiggyBankCustomization.PhotoPath) ? DefaultPaths.PiggyBankImage : PiggyBankCustomization.PhotoPath; }
        public ImageSource PiggyBankImageSource { get => _piggyBankImageSource; private set { _piggyBankImageSource = value; OnPropertyChanged(nameof(PiggyBankImageSource)); } }

        public bool IsVisible { get => PiggyBank.IsDeleted.HasValue ? !PiggyBank.IsDeleted.Value : true; }

        public bool IsEditable { get => _isEditable; private set { _isEditable = value; OnPropertyChanged(nameof(IsEditable)); } }
        public PiggyBankCustomizationDTO PiggyBankCustomization { get; private set; } = new PiggyBankCustomizationDTO();
        public UserPiggyBankDTO? UserPiggyBank { get; private set; }





        public double FillLevel
        {
            get => PiggyBank.Percentage / 100.0;
            private set { }
        }

        public string? GoalBalance
        {
            get => PiggyBank.Goal.HasValue ? PiggyBank.Goal.Value.ToString(): "∞";
            private set { }
        }

        public Command<PiggyBankDTO> MarkPiggyBankAsDeletedCommand { get; private set; }
        public Command OpenPiggyBankPageCommand { get; private set; }

        public PiggyBankCardViewModel() { Init(); }

        public PiggyBankCardViewModel(UserPiggyBankDTO? userPiggyBank, PiggyBankDTO piggyBankModel, PiggyBankCustomizationDTO piggyBankCustomizationModel, IPiggyBankService piggyBankService, IMapper mapper, INavigationService navigationService, IFileService fileService, IPopupService popupService, bool isEditable = false, bool remote = false) 
        {
            PiggyBankImageSource = DefaultPaths.PiggyBankImage;
            _piggyBankService = piggyBankService;
            PiggyBank = piggyBankModel;
            PiggyBankCustomization = piggyBankCustomizationModel;
            _mapper = mapper;
            _navigationService = navigationService;
            IsEditable = isEditable;
            UserPiggyBank = userPiggyBank;
            _remote = remote;
            _fileService = fileService;
            _popupService = popupService;
            Init();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Init()
        {
            MarkPiggyBankAsDeletedCommand = new Command<PiggyBankDTO>(async (PiggyBankDTO piggybank) => {
                try
                {
                    await DisplayDeletePiggyBankPopupAsync(piggybank);
                }
                catch (Exception ex)
                {
                    CustomExceptionHandlerMiddleware.Handle(ex);
                }
            });
            OpenPiggyBankPageCommand = new Command(async () => {
            PiggyBankPageViewModel piggyBankPageVm;
            PiggyBankPageView piggyBankPageView = null;
                using (var cts = new CancellationTokenSource())
                {
                    _popupService.ShowPopup<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cts.Token));
                    await Task.Run(async () =>
                    {
                        piggyBankPageVm = Microsoft.Maui.MauiApplication.Current.Services.GetService<PiggyBankPageViewModel>();
                        piggyBankPageView = new PiggyBankPageView(piggyBankPageVm);
                        piggyBankPageVm.ChangeDisplayMode(_isEditable, _remote);
                        piggyBankPageVm.LoadFromDTO(this.PiggyBank, this.PiggyBankCustomization, this.UserPiggyBank);

                        // piggyBankPageView.BindingContext = piggyBankPageVm;
                    });
                    await _navigationService.NavigateToAsync(piggyBankPageView);
                    cts.Cancel();
                }
               
            });
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
                    if (PiggyBankCustomization != null && PiggyBankCustomization.PhotoPath != null && _fileService.Exist(PiggyBankCustomization.PhotoPath))
                        PiggyBankImageSource = ImageSource.FromFile(PiggyBankCustomization.PhotoPath);
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
        }

        private async Task DisplayDeletePiggyBankPopupAsync(PiggyBankDTO piggyBank) 
        {
            bool answer = await App.Current.MainPage.DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить копилку?", "Да", "Нет");
            if (answer)
            {
                if (!_remote)
                {
                    var updateDto = _mapper.Map<UpdatePiggyBankDTO>(piggyBank);
                    updateDto.IsDeleted = true;
                    await _piggyBankService.UpdatePiggyBankAsync(piggyBank.Id, updateDto, CancellationToken.None, false);
                    PiggyBank.IsDeleted = true;
                }
                else
                    await _piggyBankService.DeletePiggyBankAsync(piggyBank.Id, CancellationToken.None, true);

                OnPropertyChanged(nameof(PiggyBank));
                OnPropertyChanged(nameof(IsVisible));
            }
        }



    }
}
