using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserSession.DeleteUserSession
{
    public class DeleteUserSessionCommand : IRequest<Unit>
    {
        public int UserId { get; set; }
    }
}
