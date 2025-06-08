using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class RefreshSessionDTO
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
