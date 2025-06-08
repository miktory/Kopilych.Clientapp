using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.TransactionType.CreateTransactionType
{
    public class CreateTransactionTypeCommand : IRequest<int>
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int InitiatorUserId { get; set; }

        public bool IsPositive { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
