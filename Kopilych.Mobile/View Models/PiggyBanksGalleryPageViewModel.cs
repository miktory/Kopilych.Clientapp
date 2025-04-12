using Kopilych.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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

        public ObservableCollection<PiggyBankCardViewModel> PiggyBankCardsViewModels { get; private set; }
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
        public ICommand TogglePersonalPiggyBanksVisibility { get; }
        public ICommand ToggleGroupPiggyBanksVisibility { get; }
        public event PropertyChangedEventHandler PropertyChanged;


        public PiggyBanksGalleryPageViewModel()
        {
            TogglePersonalPiggyBanksVisibility = new Command(() => { IsPersonalPiggyBanksVisible = !IsPersonalPiggyBanksVisible; });
            ToggleGroupPiggyBanksVisibility = new Command(() => { IsGroupPiggyBanksVisible = !IsGroupPiggyBanksVisible; });
            PiggyBankCardsViewModels = new ObservableCollection<PiggyBankCardViewModel>();
            var piggyBank = new PiggyBank() { Name = "Копилочка", Balance = 100, Percentage = 100 };
            var piggyBank1 = new PiggyBank() { Name = "Копилочка 1", Balance = 100, Percentage = 20 };
            var piggybankCustom = new PiggyBankCustomization() { PhotoPath = "dotnet_bot.png" };
            var piggybankCustom1 = new PiggyBankCustomization() { PhotoPath = "expand.svg" };
            PiggyBankCardsViewModels.Add(new PiggyBankCardViewModel(piggyBank, piggybankCustom));
            PiggyBankCardsViewModels.Add(new PiggyBankCardViewModel(piggyBank1, piggybankCustom1));
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
