using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.CreateFriendship
{
    public class CreateFriendshipCommand : IRequest<int>
    {
        public int InitiatorUserId { get; set; }
        public int ApproverUserId { get; set; }


    }
}
