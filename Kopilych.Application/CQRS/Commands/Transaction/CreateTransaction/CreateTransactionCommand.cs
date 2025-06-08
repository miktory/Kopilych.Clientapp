using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.Transaction.CreateTransaction
{
    public class CreateTransactionCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public int TransactionTypeId { get; set; }
        public int PaymentTypeId { get; set; }
        public int PiggyBankId { get; set; }
        public int? ExternalId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int Version {  get; set; }
        public DateTime Date { get; set; }
        public bool UpdatePiggyBankBalance { get; set; }


        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
