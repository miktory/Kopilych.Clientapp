using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.UpdateFriendship
{
    public class UpdateFriendshipCommand : IRequest
    {
        public int RequestId { get; set; }
        public bool RequestApproved { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
    }
}
