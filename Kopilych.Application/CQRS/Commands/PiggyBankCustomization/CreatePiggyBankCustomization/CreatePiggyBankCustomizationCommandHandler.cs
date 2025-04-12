using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBankCustomization.CreatePiggyBankCustomization
{
    public class CreatePiggyBankCustomizationCommandHandler : IRequestHandler<CreatePiggyBankCustomizationCommand, int>
    {
        private readonly IPiggyBankCustomizationRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IPiggyBankRepository _piggyBankRepository;
        public CreatePiggyBankCustomizationCommandHandler(IPiggyBankCustomizationRepository repository, IPiggyBankService piggyBankService)
        {
            _repository = repository;
            _piggyBankService = piggyBankService;
        }
        public async Task<int> Handle(CreatePiggyBankCustomizationCommand request, CancellationToken cancellationToken)
        {
            var piggyBankCustomization = (await _repository.GetAllForPiggyBankAsync(request.PiggyBankId, cancellationToken)).FirstOrDefault();
            if (piggyBankCustomization != null)
            {
                throw new AlreadyExistsException();
            }

            var piggyBank = await _piggyBankService.GetPiggyBankDetailsAsync(request.PiggyBankId, cancellationToken);
            var piggyBankType = await _piggyBankService.GetPiggyBankTypeDetailsAsync(request.PiggyBankTypeId, cancellationToken);

            if (!request.IsExecuteByAdmin && piggyBank.OwnerId != request.InitiatorUserId)
                throw new AccessDeniedException();

            piggyBankCustomization = new Domain.PiggyBankCustomization
            {
                PiggyBankId = request.PiggyBankId,
                PiggyBankTypeId = request.PiggyBankTypeId,
                Version = request.Version
            };

           await _repository.AddAsync(piggyBankCustomization, cancellationToken);
           
            await _repository.SaveChangesAsync(cancellationToken);

            return piggyBankCustomization.Id;
        }

    }
}
