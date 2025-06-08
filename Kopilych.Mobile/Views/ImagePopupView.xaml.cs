using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class ImagePopupView : Popup
{
	private readonly ImagePopupViewModel _vm;
    public ImagePopupView(ImagePopupViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = _vm;
	}
}