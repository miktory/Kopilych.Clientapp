using Kopilych.Shared.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserSession.GetSessionByUserId
{
    public class GetSessionByUserIdQuery : IRequest<UserSessionDTO>
    {
        public int UserId { get; set; } 
    }
}
