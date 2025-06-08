using Kopilych.Mobile.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Services
{
    internal class NavigationService : INavigationService
    {
        private readonly INavigation _navigation;

        public event EventHandler NavigationStackChanged;

        public NavigationService(INavigation navigation)
        {
            _navigation = navigation;
        }

        public async Task GoBackAsync()
        {
            if (NavigationStackCount > 1)
            {
                await _navigation.PopAsync();
     
            }
        }

        public async Task NavigateToAsync(Page page)
        {
            if (page != null)
            {
                await _navigation.PushAsync(page);
            }
        }

        public void OnNavigationStackChanged(object sender, EventArgs e)
        {
            NavigationStackChanged?.Invoke(sender, e);
        }

        public int NavigationStackCount => _navigation.NavigationStack.Count;

        
    }

}
