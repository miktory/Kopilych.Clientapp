using CommunityToolkit.Maui.Core;
using Kopilych.Application.Interfaces;
using Kopilych.Mobile.Middleware;
using Kopilych.Shared.DTO;
using Kopilych.Shared;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Kopilych.Shared.View_Models;
using AutoMapper;

namespace Kopilych.Mobile.View_Models
{
    public class PrivacyPopupViewModel : INotifyPropertyChanged
    {
        private UserPiggyBankDTO _userPiggyBank = new UserPiggyBankDTO();
        private IPiggyBankService _piggyBankService;
        private IPopupService _popupService;
        private IMapper _mapper;
        private bool _remote;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Public
        {
            get => _userPiggyBank == null ? false : _userPiggyBank.Public;
            set
            {
                if (_userPiggyBank != null)
                {
                    _userPiggyBank.Public = value;
                    OnPropertyChanged(nameof(Public));
                }
            }
        }
        public bool HideBalance
        {
            get => _userPiggyBank == null ? false : _userPiggyBank.HideBalance;
            set
            {
                if (_userPiggyBank != null)
                {
                    _userPiggyBank.HideBalance = value;
                    OnPropertyChanged(nameof(HideBalance));
                }
            }
        }

        public ICommand SaveChangesCommand { get; private set; }

        public PrivacyPopupViewModel(IPiggyBankService piggyBankService, IPopupService popupService, IMapper mapper, UserPiggyBankDTO userPiggyBank = null, bool remote = false)
        {
            _userPiggyBank = userPiggyBank;
            _popupService = popupService;
            _piggyBankService = piggyBankService;
            _mapper = mapper;
            _remote = remote;
            Init();
        }
        public PrivacyPopupViewModel() { 
        
        }

        public void LoadFromDTO(UserPiggyBankDTO userPiggyBank)
        {
            _userPiggyBank = userPiggyBank;
            PropertyInfo[] properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Проходим по каждому свойству и вызываем OnPropertyChanged
            foreach (var property in properties)
            {
                OnPropertyChanged(property.Name);
            }
        }

        public void ChangeDisplayMode(bool remote)
        {
            _remote = remote;
        }



        private void Init()
        {

            SaveChangesCommand = new Command(async () => {
                await SaveChangesAsync();
            });
        }

        private async Task SaveChangesAsync()
        {
            try
            {
              await UpdateUserPiggyBankAsync();
              await OnSave();
            }

            catch (Exception ex)
            {
                CustomExceptionHandlerMiddleware.Handle(ex);
            }
        }

        private async Task UpdateUserPiggyBankAsync()
        {
            if (_userPiggyBank == null)
                throw new NullReferenceException("UserPiggyBank is null");
            var updateUserPiggyBankDto = _mapper.Map<UpdateUserPiggyBankDTO>(_userPiggyBank);
            updateUserPiggyBankDto.Version += 1;
            await _piggyBankService.UpdateUserPiggyBankLinkAsync(_remote ? _userPiggyBank.ExternalId.Value : _userPiggyBank.Id, updateUserPiggyBankDto, CancellationToken.None, _remote);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }





        async Task OnSave()
        {
            await _popupService.ClosePopupAsync(_userPiggyBank);
        }
    }
}
