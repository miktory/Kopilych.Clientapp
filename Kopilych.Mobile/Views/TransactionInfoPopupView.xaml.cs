using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class TransactionInfoPopupView : Popup
{
    TransactionInfoPopupViewModel _vm;
    public TransactionInfoPopupView(TransactionInfoPopupViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    private void DescriptionSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        var switchElement = sender as Switch;
        if (!switchElement.IsToggled)
            _vm.FlushDescriptionCommand.Execute(null);
    }

    private void AmountEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;
        if (entry != null && (e.NewTextValue.Contains("+") || e.NewTextValue.Contains("-")))
        {
            string filtered = e.NewTextValue.Replace("+", "").Replace("-", "");
            entry.Text = filtered;
        }
    }


}