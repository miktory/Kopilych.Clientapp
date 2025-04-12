using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdateUserPiggyBankDTO
    {
        public bool? HideBalance { get; set; }
        public bool? Public { get; set; }
        public int? Version { get; set; }
    }
}
