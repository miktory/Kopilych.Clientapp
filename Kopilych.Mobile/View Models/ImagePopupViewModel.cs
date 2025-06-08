using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.View_Models
{
    public class ImagePopupViewModel: INotifyPropertyChanged
    {
        public ImageSource ImageSource { get; private set; }
        public ImagePopupViewModel()
        {
        }
        public ImagePopupViewModel(ImageSource imageSource) 
        {
            LoadImage(imageSource);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void LoadImage(ImageSource imageSource)
        {
            ImageSource = imageSource;
            OnPropertyChanged(nameof(ImageSource)); 
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
