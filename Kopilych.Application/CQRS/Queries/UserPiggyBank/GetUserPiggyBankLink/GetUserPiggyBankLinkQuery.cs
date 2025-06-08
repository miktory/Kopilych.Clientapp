using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLink
{
    public class GetUserPiggyBankLinkQuery : IRequest<UserPiggyBankDTO>
    {
        public int Id { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
