using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.View_Models
{
    public class PreloaderPopupViewModel : INotifyPropertyChanged, IDisposable
    {
        private CancellationToken _cancellationToken;
        private CancellationTokenRegistration _registration;
        private bool _isCloseRequested;
        private bool _disposed;

        public bool IsCloseRequested { get => _isCloseRequested; 
            private set { _isCloseRequested = value; OnPropertyChanged(nameof(IsCloseRequested)); } }

        public PreloaderPopupViewModel() { }


        public void SetCancellationToken(CancellationToken token)
        {
            _cancellationToken = token;

            // Регистрация обратного вызова при отмене
            if (_registration != null)
                _registration.Dispose(); // освобождение предыдущих регистраций, если нужно
            _registration = token.Register(() => IsCloseRequested = true);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                if (_registration != null)
                    _registration.Dispose();
            }

            _disposed = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
