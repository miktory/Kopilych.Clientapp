using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class PiggyBankPageView : ContentPage
{
	public PiggyBankPageView()
	{
		InitializeComponent();
		BindingContext = new PiggyBankPageViewModel();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // ����� �������� ������� �����

        // ���������� ������ ��� ���������� ����������� ����� ��� ��������� ��������
        await ((PiggyBankPageViewModel)BindingContext).LoadTransactionsAsync();
    }
}