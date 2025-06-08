using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;
using System.ComponentModel;

namespace Kopilych.Mobile.Views;

public partial class PreloaderPopupView : Popup
{
    private PreloaderPopupViewModel? _vm;
    private bool _isOpened;
    public PreloaderPopupView(PreloaderPopupViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
        this.Opened += PreloaderPopupViewModel_OnPopupOpened;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        // Если у предыдущего контекста привязки был IDisposable, освобождаем его ресурсы
        if (_vm != null )
        {
            _vm.Dispose();
            _vm = null;
        }

  


        // Получаем новый BindingContext и проверяем, реализует ли он IDisposable
        _vm = BindingContext as PreloaderPopupViewModel;
        if (_vm != null)
            _vm.PropertyChanged += PreloaderPopupViewModel_OnPropertyChanged;

        // Здесь можно добавить дополнительные действия по инициализации нового BindingContext
        // Например, обновление UI или настройка событий
    }

    private async void PreloaderPopupViewModel_OnPopupOpened(object? sender, PopupOpenedEventArgs e)
    {
        _isOpened = true;
        if (_vm.IsCloseRequested)
           await this.CloseAsync(); 
    }

    private async void PreloaderPopupViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var viewModel = (PreloaderPopupViewModel)sender;
        switch (e.PropertyName)
        {
            case $"{nameof(PreloaderPopupViewModel.IsCloseRequested)}":
                if (_isOpened)
                    Close();
                break;
        }

    }
}