using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.User.UpdateUser
{
    public class UpdateUserCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Version { get; set; }
        public string PhotoPath { get; set; }  
        public int InitiatorUserId { get; set; }
        public int? ExternalId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public bool PhotoIntegrated { get; set; }
    }
}
