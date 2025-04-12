using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Models
{
    public class PiggyBankCustomization
    {
        public int PiggyBankId { get; set; }
        public int PiggyBankTypeId { get; set; }
        public string PhotoPath { get; set; }
        public int Version { get; set; }
        public int? ExternalId { get; set; }
    }
}
