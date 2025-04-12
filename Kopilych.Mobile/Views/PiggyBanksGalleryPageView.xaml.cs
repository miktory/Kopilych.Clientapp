using Kopilych.Mobile.View_Models;
using System.ComponentModel;

namespace Kopilych.Mobile.Views;

public partial class PiggyBanksGalleryPageView : ContentPage
{
    public PiggyBanksGalleryPageView()
    {
        InitializeComponent();
        BindingContext = new PiggyBanksGalleryPageViewModel();
        ((PiggyBanksGalleryPageViewModel)BindingContext).PropertyChanged += PiggyBanksGalleryPageViewModel_OnPropertyChanged;
        PersonalPiggyBanksVisibilityImage.Rotation = 180;
        GroupPiggyBanksVisibilityImage.Rotation = 180;



    }

    private async void PiggyBanksGalleryPageViewModel_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var viewModel = (PiggyBanksGalleryPageViewModel)sender;
        var angle = 0;
        switch (e.PropertyName)
        {
            case $"{nameof(PiggyBanksGalleryPageViewModel.IsPersonalPiggyBanksVisible)}":
                angle = PersonalPiggyBanksVisibilityImage.Rotation >= 180 ? 0 : 180;
                PersonalPiggyBanksVisibilityImage.RotateTo(angle);
                break;

            case $"{nameof(PiggyBanksGalleryPageViewModel.IsGroupPiggyBanksVisible)}":
                angle = GroupPiggyBanksVisibilityImage.Rotation >= 180 ? 0 : 180;
                GroupPiggyBanksVisibilityImage.RotateTo(angle);
                break;
        }

    }
}