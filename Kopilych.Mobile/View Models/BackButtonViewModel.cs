using Android.Text;
using CommunityToolkit.Maui.Core;
using Kopilych.Mobile.Interfaces;
using Kopilych.Mobile.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kopilych.Mobile.View_Models
{
    public class BackButtonViewModel : INotifyPropertyChanged, IDisposable
    {
        private INavigationService _navigationService;
        private IPopupService _popupService;
        private bool _disposed;
        private bool _isVisible;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand GoBackCommand { get; private set; }
        public bool IsVisible { get => _isVisible; set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); } }
        public BackButtonViewModel(INavigationService navigationService, IPopupService popupService)
        {
            _popupService = popupService;
            _navigationService = navigationService;
            Init();
        }

        public BackButtonViewModel()
        {
            Init();
        }

        private void Init()
        {
            GoBackCommand = new Command(async () => {
                using (var cts = new CancellationTokenSource())
                {
                    _popupService.ShowPopup<PreloaderPopupViewModel>(popup => popup.SetCancellationToken(cts.Token));
                    if (_navigationService != null && _navigationService.NavigationStackCount > 1)
                        await _navigationService.GoBackAsync();
                    cts.Cancel();
                }

            });
            if (_navigationService != null)
                _navigationService.NavigationStackChanged += OnNavigationStackChanged;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnNavigationStackChanged(object sender, EventArgs args)
        {
            if (_navigationService != null && _navigationService.NavigationStackCount > 1)
                IsVisible = true; 
            else IsVisible = false;
      
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (_navigationService != null)
                    _navigationService.NavigationStackChanged -= OnNavigationStackChanged;
            }

            _disposed = true;
        }
    }
}
