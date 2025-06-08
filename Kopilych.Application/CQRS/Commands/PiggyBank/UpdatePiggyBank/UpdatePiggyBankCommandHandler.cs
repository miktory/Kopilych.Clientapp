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
using Kopilych.Application.Interfaces;

namespace Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank
{
    public class UpdatePiggyBankCommandHandler : IRequestHandler<UpdatePiggyBankCommand, Unit>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        public UpdatePiggyBankCommandHandler(IPiggyBankRepository repository, IPiggyBankService piggyBankService)
        {
            _repository = repository;
            _piggyBankService = piggyBankService;
        }
        public async Task<Unit> Handle(UpdatePiggyBankCommand request, CancellationToken cancellationToken)
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

            if (!request.Shared)
            {

            }
                await _piggyBankService.UnlinkAllUsersExceptOwnerAsync(piggybank.Id, cancellationToken);

            piggybank.Updated = DateTime.UtcNow;
            piggybank.Version = request.Version;
            piggybank.Balance = request.Balance;
            piggybank.Goal = request.Goal;
            piggybank.Shared = request.Shared;
            piggybank.Name = request.Name;
            piggybank.GoalDate = request.GoalDate;
            piggybank.Description = request.Description;
            piggybank.IsDeleted = request.IsDeleted;
            piggybank.ExternalId = request.ExternalId;


            await _repository.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
