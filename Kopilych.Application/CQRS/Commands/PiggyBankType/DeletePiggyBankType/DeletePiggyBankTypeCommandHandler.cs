using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByPiggyBankId;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using Kopilych.Shared;
using Kopilych.Application.Interfaces;

namespace Kopilych.Application.CQRS.Commands.PiggyBankType.DeletePiggyBankType
{
    public class DeletePiggyBankTypeCommandHandler : IRequestHandler<DeletePiggyBankTypeCommand, Unit>
    {
        private readonly IPiggyBankTypeRepository _repository;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public DeletePiggyBankTypeCommandHandler(IPiggyBankTypeRepository repository, IUserRestrictionsSettings userRestrictionsSettings)
        {
            _repository = repository;
            _userRestrictionsSettings = userRestrictionsSettings;
        }

        public async Task<Unit> Handle(DeletePiggyBankTypeCommand request, CancellationToken cancellationToken)
        {

            if (!request.IsExecuteByAdmin)
            {
                throw new AccessDeniedException();
            }

            var piggyBankType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (piggyBankType == null)
            {
                throw new NotFoundException(nameof(piggyBankType), request.Id);
            }

            await _repository.DeleteAsync(piggyBankType);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
