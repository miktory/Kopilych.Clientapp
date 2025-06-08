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
using Kopilych.Application.Services;

namespace Kopilych.Application.CQRS.Commands.Transaction.DeleteTransaction
{
    public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Unit>
    {
        private readonly ITransactionRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        private readonly IPiggyBankRepository _piggyBankRepository;
        public DeleteTransactionCommandHandler(ITransactionRepository repository, IUserRestrictionsSettings userRestrictionsSettings, IPiggyBankService piggyBankService, IPiggyBankRepository piggyBankRepository)
        {
            _repository = repository;
            _userRestrictionsSettings = userRestrictionsSettings;
            _piggyBankService = piggyBankService;
            _piggyBankRepository = piggyBankRepository;
        }

        public async Task<Unit> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (transaction == null)
            {
                throw new NotFoundException(typeof(Domain.Transaction).ToString(), request.Id);
            }

            var piggybank = transaction.PiggyBank;
            if (!request.IsExecuteByAdmin)
            {
                if (request.InitiatorUserId != piggybank.OwnerId && transaction.UserId != request.InitiatorUserId)
                    throw new AccessDeniedException();
                if (!piggybank.Shared && piggybank.OwnerId != request.InitiatorUserId)
                    throw new AccessDeniedException();
                var isMember = false;
                // являемся ли мы участником копилки
                try
                {
                    var link = await _piggyBankService.GetUserPiggyBankLinkByUserIdAndPiggyBankId(request.InitiatorUserId, piggybank.Id, cancellationToken);
                    isMember = true;
                }
                catch (NotFoundException ex)
                {

                }

                if (!isMember && piggybank.OwnerId != request.InitiatorUserId)
                    throw new AccessDeniedException();

                if (piggybank.Shared && request.UpdatePiggyBankBalance == false) // у групповых копилок всегда обновляем баланс при совершении транзакций
                    throw new AccessDeniedException();
            }

            await _repository.DeleteAsync(transaction);

            // необходимо обновлять баланс копилки здесь, т.к. у участников групповой копилки нет доступа к её обновлению напрямую
            if (request.UpdatePiggyBankBalance)
            {
                transaction.PiggyBank.Balance -= transaction.Amount;
                transaction.PiggyBank.Version += 1;
                await _piggyBankRepository.UpdateAsync(transaction.PiggyBank);
            }

            await _repository.SaveChangesAsync(cancellationToken);
            await _piggyBankRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
