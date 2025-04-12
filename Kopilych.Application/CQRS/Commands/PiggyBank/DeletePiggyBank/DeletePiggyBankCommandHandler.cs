using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.CQRS.Commands.PiggyBank.CreatePiggyBank;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;

namespace Kopilych.Application.CQRS.Commands.PiggyBank.DeletePiggyBank
{
    public class DeletePiggyBankCommandHandler : IRequestHandler<DeletePiggyBankCommand>
    {
        private readonly IPiggyBankRepository _repository;
        public DeletePiggyBankCommandHandler(IPiggyBankRepository repository) => _repository = repository;
        public async Task Handle(DeletePiggyBankCommand request, CancellationToken cancellationToken)
        {
            var piggybank = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (piggybank == null)
            {
                throw new NotFoundException(nameof(piggybank), request.Id);
            }

            if (!request.IsExecuteByAdmin)
            {
                if (piggybank.OwnerId != request.InitiatorUserId)
                    throw new AccessDeniedException();
            }

            await _repository.DeleteAsync(piggybank);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
