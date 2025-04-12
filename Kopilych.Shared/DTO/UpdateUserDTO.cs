using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdateUserDTO
    {
        public string? Username { get; set;}
        public int? Version {  get; set;}
    }
}
