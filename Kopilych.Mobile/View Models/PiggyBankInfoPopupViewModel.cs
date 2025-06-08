using AutoMapper;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Mobile.Middleware;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using Org.Apache.Http.Conn;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Xml.Serialization;

namespace Kopilych.Mobile.View_Models
{
    public partial class PiggyBankInfoPopupViewModel : INotifyPropertyChanged
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IPopupService _popupService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private ImageSource _piggyBankImageSource;
        private byte[] _newImage;
        public bool IsRemote { get => PiggyBankDetails.Shared; } 

        public PiggyBankDTO PiggyBankDetails { get; private set; }
        public PiggyBankTypeDTO SelectedPiggyBankType { get; set; }
        public PiggyBankCustomizationDTO PiggyBankCustomizationDetails { get; private set; }
        public ObservableCollection<PiggyBankTypeDTO> PiggyBankTypes { get; private set; }


        public DateTime? GoalDate { get => PiggyBankDetails.GoalDate.HasValue ? PiggyBankDetails.GoalDate.Value.ToLocalTime(): null; set { PiggyBankDetails.GoalDate = value; OnPropertyChanged(nameof(PiggyBankDetails.GoalDate)); } }
        public string PhotoPath { get => PiggyBankCustomizationDetails.PhotoPath; }
        public ImageSource PiggyBankImageSource { get => _piggyBankImageSource; private set { _piggyBankImageSource = value; OnPropertyChanged(nameof(PiggyBankImageSource)); } }

        public bool IsEditMode { get; private set; }

        public bool IsNewPiggyBank { get; private set; }

        public ObservableCollection<CarouselItem> PiggyBankStepsInfo { get; private set; }

        public partial class CarouselItem: ObservableObject
        {
            public string Title { get; set; }

            [ObservableProperty]
            private bool isElementEnabled = true;
            public Step CurrentStep { get; set; }
            public class Step 
            {
                public bool IsNameStep { get; set; }
                public bool IsDescriptionStep { get; set; }
                public bool IsGoalStep { get; set; }
                public bool IsCurrentBalanceStep { get; set; }
                public bool IsGoalDateStep { get; set; }
                public bool IsSharedStep { get; set; }
                public bool IsTypeStep { get; set; }
            }

        }

        public ICommand CloseWithNoChangesCommand { get; private set; }
        public ICommand SaveChangesCommand { get; private set; }
        public ICommand FlushDescriptionCommand { get; private set; }
        public ICommand FlushGoalDateCommand { get; private set; }
        public ICommand FlushGoalCommand { get; private set; }
        public ICommand SetDefaultGoalDateIfNullCommand { get; private set; }
        public ICommand ChangePhotoCommand { get; private set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        public PiggyBankInfoPopupViewModel(IMapper mapper, IUserInfoService userInfoService, IPopupService popupService, IPiggyBankService piggyBankService, IFileService fileService, bool editMode = false, PiggyBankCustomizationDTO piggyBankCustomizationDetails = null, PiggyBankDTO piggyBankDetails = null, bool isNewPiggyBank = true)
        {
            _mapper = mapper;
            _piggyBankService = piggyBankService;
            _userInfoService = userInfoService;
            _popupService = popupService;
            PiggyBankDetails = piggyBankDetails;
            IsEditMode = editMode;
            IsNewPiggyBank = isNewPiggyBank;
            PiggyBankCustomizationDetails = piggyBankCustomizationDetails;
            _fileService = fileService;
            if (piggyBankDetails == null) 
            {
                PiggyBankDetails = new PiggyBankDTO();
            }
            if (piggyBankCustomizationDetails == null)
            {
                PiggyBankCustomizationDetails = new PiggyBankCustomizationDTO();
            }
            Init();

        }

        private void Init()
        {
            try
            {
                PiggyBankStepsInfo = new ObservableCollection<CarouselItem>()
            {
                new CarouselItem() { Title = "Название копилки", CurrentStep = new CarouselItem.Step {IsNameStep = true } },
                new CarouselItem() { Title = "Описание копилки", CurrentStep = new CarouselItem.Step {IsDescriptionStep = true } , IsElementEnabled = !string.IsNullOrEmpty(PiggyBankDetails.Description)},
                new CarouselItem() { Title = "Текущий баланс", CurrentStep = new CarouselItem.Step { IsCurrentBalanceStep = true },  },
                new CarouselItem() { Title = "Целевой баланс", CurrentStep = new CarouselItem.Step {IsGoalStep = true } , IsElementEnabled = PiggyBankDetails.Goal.HasValue },
                new CarouselItem() { Title = "Целевая дата", CurrentStep = new CarouselItem.Step { IsGoalDateStep = true },  IsElementEnabled = PiggyBankDetails.GoalDate.HasValue },
            };
                if (IsNewPiggyBank)
                    PiggyBankStepsInfo.Add(new CarouselItem() { Title = "Тип копилки", CurrentStep = new CarouselItem.Step { IsSharedStep = true }, });
                
                PiggyBankStepsInfo.Add(new CarouselItem() { Title = "Вид копилки", CurrentStep = new CarouselItem.Step { IsTypeStep = true }, });
               


                CloseWithNoChangesCommand = new Command(async () => { await OnDiscardChanges(); });
                FlushDescriptionCommand = new Command(() => { PiggyBankDetails.Description = null; OnPropertyChanged(nameof(PiggyBankDetails)); });
                FlushGoalDateCommand = new Command(() => { PiggyBankDetails.GoalDate = null; OnPropertyChanged(nameof(PiggyBankDetails)); });
                SetDefaultGoalDateIfNullCommand = new Command(() => { PiggyBankDetails.GoalDate ??= DateTime.Today; OnPropertyChanged(nameof(PiggyBankDetails)); });
                FlushGoalCommand = new Command(() => { PiggyBankDetails.Goal = null; OnPropertyChanged(nameof(PiggyBankDetails)); });
                SaveChangesCommand = new Command(async () => { await SaveChangesAsync(); });
                ChangePhotoCommand = new Command(async () => { await LoadNewPhotoAsync(); });

                PiggyBankTypes = new ObservableCollection<PiggyBankTypeDTO>(_piggyBankService.GetAllPiggyBankTypesAsync(CancellationToken.None).GetAwaiter().GetResult().OrderBy(x=> x.Id));
                SelectedPiggyBankType = PiggyBankTypes.Where(x => x.Id == PiggyBankCustomizationDetails.PiggyBankTypeId).FirstOrDefault() ?? PiggyBankTypes.FirstOrDefault();
                UpdateAllPropertiesUI();
            }
            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);
            }
        }

        public void LoadFromDTO(PiggyBankDTO piggyBankDetails, PiggyBankCustomizationDTO piggyBankCustomizationDetails)
        {
            PiggyBankDetails = piggyBankDetails;
            PiggyBankCustomizationDetails = piggyBankCustomizationDetails;
            SelectedPiggyBankType = PiggyBankTypes.Where(x => x.Id == piggyBankCustomizationDetails.PiggyBankTypeId).FirstOrDefault() ?? PiggyBankTypes.FirstOrDefault();
            LoadDataAsync();
            //   OnPropertyChanged(nameof(_userDetails));
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

        public void ChangeDisplayMode(bool isEditMode, bool isNewPiggyBank)
        {
            IsEditMode = isEditMode;
            IsNewPiggyBank = isNewPiggyBank;
            PiggyBankStepsInfo = new ObservableCollection<CarouselItem>()
            {
                new CarouselItem() { Title = "Название копилки", CurrentStep = new CarouselItem.Step {IsNameStep = true } },
                new CarouselItem() { Title = "Описание копилки", CurrentStep = new CarouselItem.Step {IsDescriptionStep = true } , IsElementEnabled = !string.IsNullOrEmpty(PiggyBankDetails.Description)},
                new CarouselItem() { Title = "Текущий баланс", CurrentStep = new CarouselItem.Step { IsCurrentBalanceStep = true },  },
                new CarouselItem() { Title = "Целевой баланс", CurrentStep = new CarouselItem.Step {IsGoalStep = true } , IsElementEnabled = PiggyBankDetails.Goal.HasValue },
                new CarouselItem() { Title = "Целевая дата", CurrentStep = new CarouselItem.Step { IsGoalDateStep = true },  IsElementEnabled = PiggyBankDetails.GoalDate.HasValue },
            };
            if (IsNewPiggyBank)
                PiggyBankStepsInfo.Add(new CarouselItem() { Title = "Тип копилки", CurrentStep = new CarouselItem.Step { IsSharedStep = true }, });

            PiggyBankStepsInfo.Add(new CarouselItem() { Title = "Вид копилки", CurrentStep = new CarouselItem.Step { IsTypeStep = true }, });

            OnPropertyChanged(nameof(IsEditMode));
            OnPropertyChanged(nameof(PiggyBankStepsInfo));
            OnPropertyChanged(nameof(IsNewPiggyBank));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task<List<int>> CreatePiggyBankAndCustomization()
        {

            var userdata = (await _userInfoService.GetCurrentUserDetailsAsync(CancellationToken.None, false));
            PiggyBankCustomizationDetails.PiggyBankTypeId = SelectedPiggyBankType.Id;
            PiggyBankDetails.OwnerId = IsRemote ? userdata.ExternalId.Value : userdata.Id;
            var piggyBankId = await _piggyBankService.CreatePiggyBankAsync(_mapper.Map<CreatePiggyBankDTO>(PiggyBankDetails), CancellationToken.None, IsRemote);
            var customizationDTO = _mapper.Map<CreatePiggyBankCustomizationDTO>(PiggyBankCustomizationDetails);
            customizationDTO.PiggyBankId = piggyBankId;

            var customizationId = await _piggyBankService.CreatePiggyBankCustomizationAsync(customizationDTO, CancellationToken.None, IsRemote);
            var linkId = await _piggyBankService.CreateUserPiggyBankLinkAsync(new CreateUserPiggyBankDTO { ExternalId = null, HideBalance = false, Public = false, PiggyBankId = piggyBankId, UserId = PiggyBankDetails.OwnerId, Version = 0 }, CancellationToken.None, IsRemote);
            PiggyBankCustomizationDetails.Id = customizationId;
            PiggyBankDetails.Id = piggyBankId;
            PiggyBankCustomizationDetails.PiggyBankId = PiggyBankDetails.Id;
            if (_newImage != null)
                PiggyBankCustomizationDetails.PhotoPath = await _piggyBankService.UploadPiggyBankCustomizationPhotoAsync(customizationId, _newImage, CancellationToken.None, IsRemote);

            return new List<int>() {piggyBankId, customizationId, linkId};
        }

        private async Task UpdatePiggyBankAndCustomization()
        {
            PiggyBankCustomizationDetails.PiggyBankTypeId = SelectedPiggyBankType.Id;
            var updatePiggyBankDto = _mapper.Map<UpdatePiggyBankDTO>(PiggyBankDetails);
            updatePiggyBankDto.Version += 1;
            var updateCustomizationDto = _mapper.Map<UpdatePiggyBankCustomizationDTO>(PiggyBankCustomizationDetails);
            updateCustomizationDto.Version += 1;
            await _piggyBankService.UpdatePiggyBankAsync(PiggyBankDetails.Id, updatePiggyBankDto, CancellationToken.None, IsRemote);
            await _piggyBankService.UpdatePiggyBankCustomizationAsync(PiggyBankCustomizationDetails.Id, updateCustomizationDto, CancellationToken.None, IsRemote);
            if (_newImage != null)
                PiggyBankCustomizationDetails.PhotoPath = await _piggyBankService.UploadPiggyBankCustomizationPhotoAsync(PiggyBankCustomizationDetails.Id, _newImage, CancellationToken.None, IsRemote);
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                List<int> ids = null;
                if (IsNewPiggyBank)
                {
                    ids = await CreatePiggyBankAndCustomization();
                    if (ids != null)
                        await OnSave();
                }
                else
                {
                    await UpdatePiggyBankAndCustomization();
                    await OnSave();
                }
            }

            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);

            }
        }

        public async Task LoadDataAsync()
        {
            try
            {
                await LoadImage(PiggyBankDetails.Shared);
            }
            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);
            }
        }

        private async Task LoadImage(bool remote)
        {
            await Task.Run(async () =>
            {
                try
                {
                    if (PhotoPath != null)
                    {
                        if (!remote)
                        {
                            if (_fileService.Exist(PhotoPath))
                                PiggyBankImageSource = ImageSource.FromFile(PhotoPath);
                            else
                                PiggyBankImageSource = ImageSource.FromFile(DefaultPaths.PiggyBankImage);
                        }
                        else
                        {
                            byte[] image = null;

                            image = await _piggyBankService.GetPiggyBankCustomizationPhotoAsync(PiggyBankCustomizationDetails, CancellationToken.None, true);
                            PiggyBankImageSource = ImageSource.FromStream(() => new MemoryStream(image));


                        }
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

                    PiggyBankImageSource = ImageSource.FromStream(() =>
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
            await _popupService.ClosePopupAsync(Tuple.Create(PiggyBankDetails, PiggyBankCustomizationDetails));
        }

        async Task OnDiscardChanges()
        {
            await _popupService.ClosePopupAsync();
        }

    }




}
