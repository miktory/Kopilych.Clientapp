using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Interfaces
{
    public interface IUserRestrictionsSettings
    {
        public int MaxPiggyBanksCountWithPremium { get; set; }
        public int MaxPiggyBanksCountWithoutPremium { get; set; }
        public int MaxLinksPerPiggyBankCount { get; set; }
    }
}
