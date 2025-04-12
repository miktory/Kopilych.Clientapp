using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.TransactionType.GetTransactionTypeById;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByUserId;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Services;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.Transaction.GetTransactionById
{
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDTO>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPiggyBankService _piggyBankService;

        public GetTransactionByIdQueryHandler(ITransactionRepository repository, IMapper mapper, IPiggyBankService piggyBankService)
            => (_repository, _mapper, _piggyBankService) = (repository, mapper, piggyBankService);
        public async Task<TransactionDTO> Handle(GetTransactionByIdQuery request, CancellationToken cancellationToken)
        {
            var transaction = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (transaction == null)
            {
                throw new NotFoundException(typeof(Domain.Transaction).ToString(), request.Id);

            }

            var piggybank = transaction.PiggyBank;
            if (!request.IsExecuteByAdmin)
            {
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

            var vm = _mapper.Map<TransactionDTO>(transaction);
            return vm;
        }
    }
}
