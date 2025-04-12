using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetUserDetailsById
{
    public class GetUserDetailsByIdQueryValidator : AbstractValidator<GetUserDetailsByIdQuery>
    {
        public GetUserDetailsByIdQueryValidator()
        {
            RuleFor(cmd => cmd.Id).NotEmpty();
        }
    }
}
