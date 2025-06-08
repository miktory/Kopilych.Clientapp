using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserPiggyBank.CreateUserPiggyBank
{
    public class CreateUserPiggyBankCommand : IRequest<int>
    {
        public int UserId { get; set; }
        public int InitiatorUserId { get; set; }
        public int? ExternalId { get; set; }
        public int PiggyBankId { get; set; }
        public bool HideBalance { get; set; }
        public bool Public { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public int Version { get; set; }    
    }
}
