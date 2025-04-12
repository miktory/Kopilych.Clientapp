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

namespace Kopilych.Application.CQRS.Commands.Transaction.UpdateTransaction
{
    public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, Unit>
    {
        private readonly ITransactionRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IPiggyBankRepository _piggyBankRepository;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        private readonly ITransactionService _transactionService;

        public UpdateTransactionCommandHandler(ITransactionRepository repository, IPiggyBankRepository piggyBankRepository, IUserRestrictionsSettings userRestrictionsSettings, IPiggyBankService piggyBankService, ITransactionService transactionService)
        {
            _repository = repository;
            _userRestrictionsSettings = userRestrictionsSettings;
            _piggyBankService = piggyBankService;
            _transactionService = transactionService;
            _piggyBankRepository = piggyBankRepository;
        }

        public async Task<Unit> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (transaction == null)
            {
                throw new NotFoundException(typeof(Domain.Transaction).ToString(), request.Id);
            }

            var piggybank = transaction.PiggyBank;
            if (!request.IsExecuteByAdmin)
            {
                if (request.InitiatorUserId != transaction.UserId)
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
            }

            var transactionType = await _transactionService.GetTransactionTypeDetailsAsync(request.TransactionTypeId, cancellationToken);
            var paymentType = await _transactionService.GetPaymentTypeDetailsAsync(request.PaymentTypeId, cancellationToken);

            transaction.PiggyBank.Balance -= transaction.Amount;

            transaction.Updated = DateTime.UtcNow;
            transaction.Version = request.Version;
            transaction.Amount = request.Amount;
            transaction.Date = request.Date;
            transaction.PaymentTypeId = paymentType.Id;
            transaction.TransactionTypeId = transactionType.Id;
            transaction.Description = request.Description;

            transaction.PiggyBank.Balance += transaction.Amount;

            await _piggyBankRepository.UpdateAsync(transaction.PiggyBank);
            await _repository.UpdateAsync(transaction);
            await _repository.SaveChangesAsync(cancellationToken);
            await _piggyBankRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
