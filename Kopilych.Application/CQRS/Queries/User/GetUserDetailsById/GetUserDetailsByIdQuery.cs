using Kopilych.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetUserDetailsById
{
    public class GetUserDetailsByIdQuery : IRequest<UserDetailsDTO>
    {
        public int Id { get; set; }
        public int InitiatorUserId { get; set;}
        public bool IsExecuteByAdmin { get; set; }
    }
}
