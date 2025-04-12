using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetLinksToCommonPiggyBank
{
    public class GetCommonPiggyBanksIdsForUsersQuery : IRequest<List<int>>
    {
        public int FirstUserId { get; set; }
        public int SecondUserId { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
