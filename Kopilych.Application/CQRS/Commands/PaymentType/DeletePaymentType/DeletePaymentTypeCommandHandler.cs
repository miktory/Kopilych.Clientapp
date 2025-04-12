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

namespace Kopilych.Application.CQRS.Commands.PaymentType.DeletePaymentType
{
    public class DeletePaymentTypeCommandHandler : IRequestHandler<DeletePaymentTypeCommand, Unit>
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public DeletePaymentTypeCommandHandler(IPaymentTypeRepository repository, IUserRestrictionsSettings userRestrictionsSettings)
        {
            _repository = repository;
            _userRestrictionsSettings = userRestrictionsSettings;
        }

        public async Task<Unit> Handle(DeletePaymentTypeCommand request, CancellationToken cancellationToken)
        {

            if (!request.IsExecuteByAdmin)
            {
                throw new AccessDeniedException();
            }

            var paymentType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (paymentType == null)
            {
                throw new NotFoundException(nameof(paymentType), request.Id);
            }

            await _repository.DeleteAsync(paymentType);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
