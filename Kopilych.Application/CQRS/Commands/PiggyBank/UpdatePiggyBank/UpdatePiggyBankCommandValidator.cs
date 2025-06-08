using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank
{
    public class UpdatePiggyBankCommandValidator : AbstractValidator<UpdatePiggyBankCommand>
    {
        public UpdatePiggyBankCommandValidator()
        {
          //  RuleFor(cmd => cmd.Balance).GreaterThanOrEqualTo(0);
            RuleFor(cmd => cmd.Goal).GreaterThanOrEqualTo(0);
            RuleFor(cmd => cmd.Name).NotEmpty(); 
        }
    }
}
