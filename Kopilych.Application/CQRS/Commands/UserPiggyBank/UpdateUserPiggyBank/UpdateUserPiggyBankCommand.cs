using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserPiggyBank.UpdateUserPiggyBank
{
    public class UpdateUserPiggyBankCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int InitiatorUserId { get; set; }
        public bool HideBalance { get; set; }
        public int? ExternalId { get; set; }
        public bool Public { get; set; }
        public int Version {  get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
