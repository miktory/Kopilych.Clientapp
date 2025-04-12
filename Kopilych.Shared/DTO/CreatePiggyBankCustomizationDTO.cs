using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreatePiggyBankCustomizationDTO
    {
        [Required]
        public int PiggyBankId { get; set; }
        [Required]
        public int PiggyBankTypeId { get; set; }
        [Required]
        public int Version { get; set; }
    }
}
