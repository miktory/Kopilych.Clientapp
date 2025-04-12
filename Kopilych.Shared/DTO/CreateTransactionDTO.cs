using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreateTransactionDTO
    {
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
       
        public string? Description { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }
}
