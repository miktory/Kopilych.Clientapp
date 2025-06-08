using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Interfaces
{
    public interface INavigationService
    {
        Task GoBackAsync();
        Task NavigateToAsync(Page page);
        int NavigationStackCount { get; }
        event EventHandler NavigationStackChanged;
        public void OnNavigationStackChanged(object sender, EventArgs e); // это будет вызывать shell
    }
}
