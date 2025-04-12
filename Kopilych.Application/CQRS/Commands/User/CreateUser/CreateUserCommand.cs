using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.User.CreateUser
{
    public class CreateUserCommand : IRequest<int>
    {
        public Guid ExternalUserGuid { get; set; }
        public string Username { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public int InitiatorUserId { get; set; }
    }
}
