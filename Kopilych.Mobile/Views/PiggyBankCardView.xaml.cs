using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetLinksToCommonPiggyBank;
using Kopilych.Mobile.View_Models;
using Microsoft.Extensions.DependencyInjection;

namespace Kopilych.Mobile.Views;

public partial class PiggyBankCardView : ContentView
{
    private PiggyBankCardViewModel _vm;
    public PiggyBankCardView()
	{
        InitializeComponent();
        BindingContext = new PiggyBankCardViewModel();
        //	BalancePercentage.WidthRequest = Card.WidthRequest;
        //   BalancePercentage.HeightRequest = Card.HeightRequest;
    }

    public PiggyBankCardView(PiggyBankCardViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;


    }

    protected override async void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        _vm = BindingContext as PiggyBankCardViewModel;
        if (_vm != null)
        {
            Task.Run(async () => await _vm.LoadData());

        }
    }
}