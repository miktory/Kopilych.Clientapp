using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class PiggyBankInfoPopupView : Popup
{
	private readonly PiggyBankInfoPopupViewModel _vm;

    public PiggyBankInfoPopupView(PiggyBankInfoPopupViewModel vm)
	{
		InitializeComponent();
		_vm = vm;
		BindingContext = _vm;
        _vm.LoadDataAsync();
	}

    private void ButtonNext_Clicked(object sender, EventArgs e)
    {
        var items = _vm.PiggyBankStepsInfo;

        int currentIndex = items.IndexOf(PiggyBankStepsCarousel.CurrentItem as PiggyBankInfoPopupViewModel.CarouselItem);
        // Переход к следующему элементу

        if (currentIndex < items.Count - 1)
        {
            PiggyBankStepsCarousel.ScrollTo(currentIndex + 1); // Переход к следующему элементу с анимацией
        }
        //else
        //{
        //    await carouselView.ScrollTo(0); // Возврат к первому элементу
        //}
    }

    private void ButtonPrevious_Clicked(object sender, EventArgs e)
    {
        var items = _vm.PiggyBankStepsInfo;

        int currentIndex = items.IndexOf(PiggyBankStepsCarousel.CurrentItem as PiggyBankInfoPopupViewModel.CarouselItem);
        // Переход к следующему элементу

        if (currentIndex > 0)
        {
            PiggyBankStepsCarousel.ScrollTo(currentIndex - 1); // Переход к предыдущему элементу с анимацией
        }
    }

    private void DescriptionSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var switchElement = sender as Switch;
        if (!switchElement.IsToggled)
            _vm.FlushDescriptionCommand.Execute(null);
    }

    private void GoalSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var switchElement = sender as Switch;
        if (!switchElement.IsToggled)
            _vm.FlushGoalCommand.Execute(null);
    }

    private void GoalDateSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var switchElement = sender as Switch;
        if (!switchElement.IsToggled)
            _vm.FlushGoalDateCommand.Execute(null);
        else
            _vm.SetDefaultGoalDateIfNullCommand.Execute(null);
    }

    private void CurrentBalanceEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;
        try
        {
            if (decimal.Parse(entry.Text) < 0)
                entry.Text = "0";
        }
        catch { }
    }
}