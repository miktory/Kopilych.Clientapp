using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class PiggyBankCustomizationDTO
    {
        public int Id {  get; set; }
        public int PiggyBankId { get; set; }    
        public int PiggyBankTypeId { get; set; }
        public int Version { get; set; }
    }
}
