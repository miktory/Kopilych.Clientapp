using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;
using System.Runtime.CompilerServices;

namespace Kopilych.Mobile.Views;

public partial class CreateOrEditUserPopupView : Popup
{
    private readonly CreateOrEditUserPopupViewModel _vm;

    public CreateOrEditUserPopupView(CreateOrEditUserPopupViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
        _vm.LoadDataAsync();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
    }

    private async void ClosePopupButton_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync();
    }

}