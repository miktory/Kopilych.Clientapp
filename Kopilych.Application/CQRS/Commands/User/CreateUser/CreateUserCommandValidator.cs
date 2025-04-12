using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.User.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(cmd => cmd.ExternalUserGuid).NotEmpty().NotEqual(Guid.Empty);
            RuleFor(cmd => cmd.Username).NotEmpty().MaximumLength(50);
        }
    }
}
