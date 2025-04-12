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

namespace Kopilych.Application.CQRS.Commands.Transaction.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
    {
        private readonly ITransactionRepository _repository;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IPiggyBankRepository _piggyBankRepository;
        private readonly ITransactionService _transactionService;
        private readonly IUserInfoService _userInfoService;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public CreateTransactionCommandHandler(ITransactionRepository repository, IUserRestrictionsSettings userRestrictionsSettings, IPiggyBankService piggyBankService, ITransactionService transactionService, IUserInfoService userInfoService, IPiggyBankRepository piggyBankRepository)
        {
            _repository = repository;
            _userRestrictionsSettings = userRestrictionsSettings;
            _piggyBankService = piggyBankService;
            _transactionService = transactionService;
            _userInfoService = userInfoService;
            _piggyBankRepository = piggyBankRepository;
        }

        public async Task<int> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var piggybank = await _piggyBankService.GetPiggyBankDetailsAsync(request.PiggyBankId, cancellationToken);
            var transactionType = await _transactionService.GetTransactionTypeDetailsAsync(request.TransactionTypeId, cancellationToken);
            var paymentType = await _transactionService.GetPaymentTypeDetailsAsync(request.PaymentTypeId, cancellationToken);
            var user = await _userInfoService.GetUserDetailsAsync(request.UserId, cancellationToken);

            if (!request.IsExecuteByAdmin)
            {
                if (request.UserId != request.InitiatorUserId)
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

            var transaction = new Domain.Transaction
            {
                Date = request.Date,
                Description = request.Description,
                Amount = request.Amount,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                PaymentTypeId = request.PaymentTypeId,
                TransactionTypeId = request.TransactionTypeId,
                Version = request.Version,
                UserId = request.UserId,
                PiggyBankId = request.PiggyBankId,
            };            

            await _repository.AddAsync(transaction, cancellationToken);

            // необходимо обновлять баланс копилки здесь, т.к. у участников групповой копилки нет доступа к её обновлению напрямую

            var pb = await _piggyBankRepository.GetByIdAsync(request.PiggyBankId, cancellationToken);
            pb.Balance += transaction.Amount;
            await _piggyBankRepository.UpdateAsync(pb);

            await _repository.SaveChangesAsync(cancellationToken);
            await _piggyBankRepository.SaveChangesAsync(cancellationToken);
            return transaction.Id;




        }
    }
}
