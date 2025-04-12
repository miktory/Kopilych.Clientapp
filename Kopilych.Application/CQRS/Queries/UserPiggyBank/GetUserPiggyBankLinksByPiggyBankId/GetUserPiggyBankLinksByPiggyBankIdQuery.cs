using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByPiggyBankId
{
    public class GetUserPiggyBankLinksByPiggyBankIdQuery : IRequest<List<UserPiggyBankVm>>
    {
        public int PiggyBankId { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
