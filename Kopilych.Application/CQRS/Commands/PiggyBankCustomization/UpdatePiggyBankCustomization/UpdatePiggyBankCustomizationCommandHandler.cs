using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBankCustomization.UpdatePiggyBankCustomization
{
    public class UpdatePiggyBankCustomizationCommandHandler : IRequestHandler<UpdatePiggyBankCustomizationCommand, Unit>
    {
        private readonly IPiggyBankCustomizationRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IPiggyBankRepository _piggyBankRepository;
        public UpdatePiggyBankCustomizationCommandHandler(IPiggyBankCustomizationRepository repository, IPiggyBankService piggyBankService)
        {
            _repository = repository;
            _piggyBankService = piggyBankService;
        }
        public async Task<Unit> Handle(UpdatePiggyBankCustomizationCommand request, CancellationToken cancellationToken)
        {
            var piggyBankCustomization = (await _repository.GetByIdAsync(request.Id, cancellationToken));
            if (piggyBankCustomization == null)
            {
                throw new NotFoundException(nameof(piggyBankCustomization), request.Id);
            }

            if (!request.IsExecuteByAdmin && piggyBankCustomization.PiggyBank.OwnerId != request.InitiatorUserId)
                throw new AccessDeniedException();

            var piggyBankType = await _piggyBankService.GetPiggyBankTypeDetailsAsync(request.PiggyBankTypeId, cancellationToken);

            piggyBankCustomization.Version = request.Version;
            piggyBankCustomization.PiggyBankTypeId = request.PiggyBankTypeId;
            piggyBankCustomization.PhotoPath = request.PhotoPath;
            piggyBankCustomization.ExternalId = request.ExternalId;
            piggyBankCustomization.PhotoIntegrated = request.PhotoIntegrated;

            await _repository.UpdateAsync(piggyBankCustomization);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
