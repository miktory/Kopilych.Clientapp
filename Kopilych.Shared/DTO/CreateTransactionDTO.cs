using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kopilych.Shared.DTO
{
    public class CreateTransactionDTO
    {
        private DateTime _date;

        [Required]
        public int UserId { get; set; }
        [Required]
        public int TransactionTypeId { get; set; }
        [Required]
        public int PaymentTypeId { get; set; }
        [Required]
        public int Version { get; set; }
        [Required]
        public int PiggyBankId { get; set; }
        [Required]
        public decimal Amount { get; set; }

        [JsonIgnore]
        public int? ExternalId { get; set; }

        public string? Description { get; set; }
        [Required]
        public DateTime Date
        {
            get => _date.ToUniversalTime();
            set
            {
                _date = value.ToUniversalTime();
            }
        }
    }
}
