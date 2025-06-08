using AndroidX.Lifecycle;
using CommunityToolkit.Maui.Views;
using Kopilych.Mobile.View_Models;

namespace Kopilych.Mobile.Views;

public partial class FriendlistPopupView : ContentPage
{
    private FriendlistPopupViewModel _vm;
    public FriendlistPopupView(FriendlistPopupViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
  
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // ����� �������� ������� �����
        NavigationPage.SetHasNavigationBar(this, false);

        _vm.LoadDataAsync(CancellationToken.None);
        // ���������� ������ ��� ���������� ����������� ����� ��� ��������� ��������
    }

}