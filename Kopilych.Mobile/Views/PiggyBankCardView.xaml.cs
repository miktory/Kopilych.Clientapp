using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class PiggyBankCardView : ContentView
{
	public PiggyBankCardView()
	{
		InitializeComponent();
		BalancePercentage.WidthRequest = Card.WidthRequest;
        BalancePercentage.HeightRequest = Card.HeightRequest;
    }
}