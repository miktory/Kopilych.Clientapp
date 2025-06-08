using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Kopilych.Shared.DTO
{
    public class TransactionDTO
    {
        private DateTime _date;

        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int TransactionTypeId { get; set; }
        [Required]
        public int PaymentTypeId { get; set; }
        [Required]
        public int PiggyBankId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        [Required]
        public DateTime Date 
        { 
            get => _date.ToUniversalTime();
            set {
                _date = value.ToUniversalTime(); 
            }
        }
        [Required]
        public int Version { get; set; }

        [JsonIgnore]
        public bool? IsDeleted { get; set; }

        [JsonIgnore]
        public int? ExternalId { get; set; }


        public object Clone()
        {
            return new TransactionDTO { Id = Id, ExternalId = ExternalId, Date = Date, Amount = Amount, Description = Description, IsDeleted = IsDeleted, PaymentTypeId = PaymentTypeId, PiggyBankId = PiggyBankId, TransactionTypeId = TransactionTypeId, UserId = UserId, Version = Version };
        }
    }
}
