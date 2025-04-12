using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
	public class Transaction
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int TransactionTypeId { get; set; }
		public int PaymentTypeId { get; set; }
		public int PiggyBankId { get; set; }
		public decimal Amount { get; set; }
		public string? Description { get; set; }
		public DateTime Date { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
        public int Version { get; set; }
        public virtual User User { get; set; }
		public virtual TransactionType TransactionType { get; set; }
		public virtual PaymentType PaymentType { get; set; }
		public virtual PiggyBank PiggyBank { get; set; }
	}
}
