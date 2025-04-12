using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBankCustomization.DeletePiggyBankCustomization
{
    public class DeletePiggyBankCustomizationCommandHandler : IRequestHandler<DeletePiggyBankCustomizationCommand, Unit>
    {
        private readonly IPiggyBankCustomizationRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IPiggyBankRepository _piggyBankRepository;
        public DeletePiggyBankCustomizationCommandHandler(IPiggyBankCustomizationRepository repository, IPiggyBankService piggyBankService)
        {
            _repository = repository;
            _piggyBankService = piggyBankService;
        }
        public async Task<Unit> Handle(DeletePiggyBankCustomizationCommand request, CancellationToken cancellationToken)
        {
            var piggyBankCustomization = (await _repository.GetByIdAsync(request.Id, cancellationToken));
            if (piggyBankCustomization == null)
            {
                throw new NotFoundException(nameof(piggyBankCustomization), request.Id);
            }

            if (!request.IsExecuteByAdmin && piggyBankCustomization.PiggyBank.OwnerId != request.InitiatorUserId)
                throw new AccessDeniedException();

            await _repository.DeleteAsync(piggyBankCustomization);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
