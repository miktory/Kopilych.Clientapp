using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.DeleteFriendship
{
    public class DeleteFriendshipCommand : IRequest
    {
        public int Id { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecutedByAdmin { get; set; }
    }
}
