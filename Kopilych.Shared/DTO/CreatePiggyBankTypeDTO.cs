using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreatePaymentTypeDTO
    {
        [Required]
        public string Name { get; set; }

    }
}
