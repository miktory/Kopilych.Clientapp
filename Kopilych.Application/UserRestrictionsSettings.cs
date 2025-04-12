using Kopilych.Application.Interfaces;

namespace Kopilych.Application
{
    public class UserRestrictionsSettings: IUserRestrictionsSettings
    {
        public int  MaxPiggyBanksCountWithPremium { get; set; }
        public int MaxPiggyBanksCountWithoutPremium { get; set; }
        public int MaxLinksPerPiggyBankCount { get; set; }
    }
}
