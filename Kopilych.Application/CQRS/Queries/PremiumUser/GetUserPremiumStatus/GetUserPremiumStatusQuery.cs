using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus
{
    public class GetUserPremiumStatusQuery : IRequest<PremiumStatusVm>
    {
        public int UserId { get; set; }
    }
}
