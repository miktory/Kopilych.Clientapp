using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class TransactionView : ContentView
{
	private TransactionViewModel _vm;
	public TransactionView()
	{
		InitializeComponent();
	}



    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
		_vm = BindingContext as TransactionViewModel;
		if (_vm != null ) 
		{
			_vm.Init();
			Task.Run(async () => await _vm.LoadData());

		}
    }

}