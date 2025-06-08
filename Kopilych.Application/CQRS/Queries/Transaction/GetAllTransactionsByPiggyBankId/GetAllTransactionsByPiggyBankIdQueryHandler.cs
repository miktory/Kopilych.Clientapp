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

namespace Kopilych.Application.CQRS.Queries.Transaction.GetAllTransactionsByPiggyBankId
{
    public class GetAllTransactionsByPiggyBankIdQueryHandler : IRequestHandler<GetAllTransactionsByPiggyBankIdQuery, List<TransactionDTO>>
    {
        private readonly ITransactionRepository _repository;
        private readonly IMapper _mapper;
        private readonly IPiggyBankService _piggyBankService;

        public GetAllTransactionsByPiggyBankIdQueryHandler(ITransactionRepository repository, IMapper mapper, IPiggyBankService piggyBankService)
            => (_repository, _mapper, _piggyBankService) = (repository, mapper, piggyBankService);
        public async Task<List<TransactionDTO>> Handle(GetAllTransactionsByPiggyBankIdQuery request, CancellationToken cancellationToken)
        {
            
            var piggybank = await _piggyBankService.GetPiggyBankDetailsAsync(request.PiggyBankId, cancellationToken, false);

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

            var transactions = await _repository.GetAllForPiggyBankAsync(request.PiggyBankId, cancellationToken);
            if (transactions == null)
            {
                throw new NotFoundException(nameof(transactions), "none");
            }

            var result = new List<TransactionDTO>();
            foreach (var t in transactions)
                result.Add(_mapper.Map<TransactionDTO>(t));
            return result;
        }
    }
}
