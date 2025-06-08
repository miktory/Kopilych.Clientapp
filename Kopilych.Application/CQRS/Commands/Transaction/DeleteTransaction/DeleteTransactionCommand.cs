using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.Transaction.DeleteTransaction
{
    public class DeleteTransactionCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public bool UpdatePiggyBankBalance { get; set; }

    }
}
