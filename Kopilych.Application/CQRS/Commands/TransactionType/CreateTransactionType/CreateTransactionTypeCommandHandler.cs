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

namespace Kopilych.Application.CQRS.Commands.TransactionType.CreateTransactionType
{
    public class CreateTransactionTypeCommandHandler : IRequestHandler<CreateTransactionTypeCommand, int>
    {
        private readonly ITransactionTypeRepository _repository;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public CreateTransactionTypeCommandHandler(ITransactionTypeRepository repository, IUserRestrictionsSettings userRestrictionsSettings)
        {
            _repository = repository;
            _userRestrictionsSettings = userRestrictionsSettings;
        }

        public async Task<int> Handle(CreateTransactionTypeCommand request, CancellationToken cancellationToken)
        {

            if (!request.IsExecuteByAdmin)
            {
                throw new AccessDeniedException();
            }

            var tt = new Domain.TransactionType
            {
                Name = request.Name,
            };

            await _repository.AddAsync(tt, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return tt.Id;
        }
    }
}
