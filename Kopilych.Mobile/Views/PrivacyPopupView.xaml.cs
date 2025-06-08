using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class PrivacyPopupView : Popup
{
    private PrivacyPopupViewModel _vm;
	public PrivacyPopupView(PrivacyPopupViewModel vm)
	{
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

}