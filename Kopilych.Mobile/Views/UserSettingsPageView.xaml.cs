using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class UserSettingsPageView : ContentPage
{
	private UserSettingsPageViewModel _vm;
	public UserSettingsPageView(UserSettingsPageViewModel vm)
	{
		_vm = vm;
		BindingContext = _vm;
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        NavigationPage.SetHasNavigationBar(this, false);
        _vm.LoadDataAsync(); // Вызываем асинхронный метод
    }


}