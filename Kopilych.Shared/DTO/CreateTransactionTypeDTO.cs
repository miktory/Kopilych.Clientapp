using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreateTransactionTypeDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string IsPositive { get; set; }
    }
}
