using Kopilych.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId
{
    public class GetUserDetailsByExternalIdQuery : IRequest<UserDetailsDTO>
    {
        public int ExternalId { get; set; }
        // информацию по гуидам получает система. пользователю это не нужно. делать проверки доступа пока не вижу смысла.
    }
}
