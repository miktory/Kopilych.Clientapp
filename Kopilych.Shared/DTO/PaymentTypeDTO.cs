using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class PaymentTypeDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name {  get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is PaymentTypeDTO dto && Name == dto.Name && Id == dto.Id;
        }

        public override int GetHashCode()
        {
            return (Name + Id.ToString()).GetHashCode();
        }
    }
}
