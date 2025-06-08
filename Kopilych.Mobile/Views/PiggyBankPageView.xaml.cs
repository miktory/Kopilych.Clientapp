using Kopilych.Mobile.View_Models;
using System.ComponentModel;
using System.Runtime.Intrinsics.X86;

namespace Kopilych.Mobile.Views;

public partial class PiggyBankPageView : ContentPage
{
    private PiggyBankPageViewModel _vm;

    public PiggyBankPageView(PiggyBankPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    public void Init()
    {
        _vm.PropertyChanged += PiggyBankPageViewModel_OnPropertyChanged;
    }

    private async void PiggyBankPageViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var viewModel = (PiggyBankPageViewModel)sender;
        var angle = 0;
        switch (e.PropertyName)
        {
            case $"{nameof(PiggyBankPageViewModel.IsPiggyBankDeleted)}":
                if (viewModel.PiggyBank.IsDeleted.HasValue && viewModel.PiggyBank.IsDeleted.Value == true)
                    await Navigation.PopAsync();
                break;
        }

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // Важно вызывать базовый метод
        NavigationPage.SetHasNavigationBar(this, false);
        // Обновление данных или выполнение асинхронных задач при появлении страницы
        _vm.LoadDataAsync();
        _vm.ConfigureUiAsync();
    }

 
}