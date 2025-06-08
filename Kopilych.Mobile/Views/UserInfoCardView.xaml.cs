using AndroidX.Lifecycle;
using Java.Lang;
using Kopilych.Domain;
using Kopilych.Mobile.View_Models;
using Microsoft.Maui.Controls.Internals;
using static Kopilych.Mobile.View_Models.UserInfoCardViewModel;

namespace Kopilych.Mobile.Views;

public partial class UserInfoCardView : ContentView
{

    private UserInfoCardViewModel _vm;

    public UserInfoCardView()
    {
        InitializeComponent();

    }

    public UserInfoCardView(UserInfoCardViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        // Здесь ваш код, который будет выполняться при изменении BindingContext

        _vm = this.BindingContext as UserInfoCardViewModel;
        _vm.LoadDataAsync();
    }

}