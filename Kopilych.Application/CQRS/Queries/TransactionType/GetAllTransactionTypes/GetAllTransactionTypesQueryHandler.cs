using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.TransactionType.GetTransactionTypeById;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByUserId;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
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

namespace Kopilych.Application.CQRS.Queries.TransactionType.GetAllTransactionTypes
{
    public class GetAllTransactionTypesQueryHandler : IRequestHandler<GetAllTransactionTypesQuery, List<TransactionTypeDTO>>
    {
        private readonly ITransactionTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllTransactionTypesQueryHandler(ITransactionTypeRepository repository, IMapper mapper)
            => (_repository, _mapper) = (repository, mapper);
        public async Task<List<TransactionTypeDTO>> Handle(GetAllTransactionTypesQuery request, CancellationToken cancellationToken)
        {
            var transactionTypes = await _repository.GetAllAsync(cancellationToken);
            if (transactionTypes == null)
            {
                throw new NotFoundException(nameof(transactionTypes), "none");
            }
            
            var result = new List<TransactionTypeDTO>();
            foreach (var pt in transactionTypes)
                result.Add(_mapper.Map<TransactionTypeDTO>(pt));
            return result;
        }
    }
}
