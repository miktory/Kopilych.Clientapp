using Kopilych.Mobile.View_Models;
using System.ComponentModel;

namespace Kopilych.Mobile.Views;

public partial class PiggyBanksGalleryPageView : ContentPage
{
    private PiggyBanksGalleryPageViewModel _vm;
    public PiggyBanksGalleryPageView(PiggyBanksGalleryPageViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
        ((PiggyBanksGalleryPageViewModel)BindingContext).PropertyChanged += PiggyBanksGalleryPageViewModel_OnPropertyChanged;
        PersonalPiggyBanksVisibilityImage.Rotation = 180;
        GroupPiggyBanksVisibilityImage.Rotation = 180;
    }

    private async void PiggyBanksGalleryPageViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var viewModel = (PiggyBanksGalleryPageViewModel)sender;
        switch (e.PropertyName)
        {
            case $"{nameof(PiggyBanksGalleryPageViewModel.IsPersonalPiggyBanksVisible)}":
               if (_vm.IsPersonalPiggyBanksVisible)
                    PersonalPiggyBanksVisibilityImage.RotateTo(0);
               else
                    PersonalPiggyBanksVisibilityImage.RotateTo(180);
                break;

            case $"{nameof(PiggyBanksGalleryPageViewModel.IsGroupPiggyBanksVisible)}":
                if (_vm.IsGroupPiggyBanksVisible)
                    GroupPiggyBanksVisibilityImage.RotateTo(0);
                else
                    GroupPiggyBanksVisibilityImage.RotateTo(180);
                break;
        }

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        NavigationPage.SetHasNavigationBar(this, false);
        await _vm.StartNewbieHelperAsync(CancellationToken.None);
        _vm.LoadDataAsync(CancellationToken.None); // Вызываем асинхронный метод
    }
}