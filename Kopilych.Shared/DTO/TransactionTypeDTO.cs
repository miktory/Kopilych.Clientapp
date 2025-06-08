using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class TransactionTypeDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name {  get; set; }

        [Required, JsonIgnore]
        public bool IsPositive { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            return obj is TransactionTypeDTO dto && Name == dto.Name && Id == dto.Id && IsPositive == dto.IsPositive;
        }

        public override int GetHashCode()
        {
            return (Name + Id.ToString() + IsPositive.ToString()).GetHashCode();
        }
    }
}
