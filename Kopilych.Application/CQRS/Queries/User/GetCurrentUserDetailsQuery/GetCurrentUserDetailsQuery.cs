using Kopilych.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetCurrentUserDetails
{
    public class GetCurrentUserDetailsQuery : IRequest<UserDetailsDTO>
    {

    }
}
