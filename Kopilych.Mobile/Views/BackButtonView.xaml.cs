using AndroidX.Lifecycle;
using Kopilych.Mobile.View_Models;
using System.ComponentModel;

namespace Kopilych.Mobile.Views;

public partial class BackButtonView : ContentView
{
    private BackButtonViewModel _vm;
	public BackButtonView()
	{
		InitializeComponent();
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

    }
}