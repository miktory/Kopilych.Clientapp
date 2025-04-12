using Kopilych.Mobile.Models;
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
        private PiggyBank _piggyBank;
        private PiggyBankCustomization _piggyBankCustomization;

        public string Name
        {
            get => _piggyBank.Name;
            private set { }
        }

        public double FillLevel
        {
            get => _piggyBank.Percentage / 100.0 ;
            private set { }
        }

        public string PhotoPath
        {
            get => _piggyBankCustomization.PhotoPath;
            private set { }
        }
        public PiggyBankCardViewModel() { }

        public PiggyBankCardViewModel(PiggyBank piggyBankModel, PiggyBankCustomization piggyBankCustomizationModel) 
        {
            _piggyBank = piggyBankModel;
            _piggyBankCustomization = piggyBankCustomizationModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

          protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
