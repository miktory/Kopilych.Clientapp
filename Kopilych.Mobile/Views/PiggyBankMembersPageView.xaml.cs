using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class PiggyBankMembersPageView : ContentPage
{
	public PiggyBankMembersPageView()
	{
		InitializeComponent();
	}

    private PiggyBankMembersPageViewModel _vm;
    public PiggyBankMembersPageView(PiggyBankMembersPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;

    }

    //protected override void OnBindingContextChanged()
    //{
    //    base.OnBindingContextChanged();
    //    _vm = BindingContext as PiggyBankMembersPageViewModel;
    //}

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // Важно вызывать базовый метод
        NavigationPage.SetHasNavigationBar(this, false);
        _vm.LoadMembersAsync(CancellationToken.None);
        // Обновление данных или выполнение асинхронных задач при появлении страницы
    }
}