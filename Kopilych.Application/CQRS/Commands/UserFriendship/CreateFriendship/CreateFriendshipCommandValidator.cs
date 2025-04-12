using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.CreateFriendship
{
    public class CreateFriendshipCommandValidator : AbstractValidator<CreateFriendshipCommand>
    {
        public CreateFriendshipCommandValidator()
        {
            RuleFor(cmd => cmd.InitiatorUserId).NotEmpty();
            RuleFor(cmd => cmd.ApproverUserId).NotEmpty();
            RuleFor(cmd => cmd.InitiatorUserId).NotEqual(cmd => cmd.ApproverUserId);
            RuleFor(cmd => cmd.InitiatorUserId).GreaterThan(-1);
            RuleFor(cmd => cmd.ApproverUserId).GreaterThan(-1);
        }
    }
}
