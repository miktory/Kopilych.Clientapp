using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId
{
    public class GetUserDetailsByExternalIdQueryValidator : AbstractValidator<GetUserDetailsByExternalIdQuery>
    {
        public GetUserDetailsByExternalIdQueryValidator()
        {
            RuleFor(cmd => cmd.ExternalId).NotEmpty();
        }
    }
}
