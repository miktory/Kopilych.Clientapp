using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.View_Models
{
    public class PremiumStatusVm
    {
        public bool Premium { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
