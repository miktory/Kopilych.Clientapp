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

namespace Kopilych.Application.CQRS.Queries.TransactionType.GetTransactionTypeById
{
    public class GetTransactionTypeByIdQueryHandler : IRequestHandler<GetTransactionTypeByIdQuery, TransactionTypeDTO>
    {
        private readonly ITransactionTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetTransactionTypeByIdQueryHandler(ITransactionTypeRepository repository, IMapper mapper)
            => (_repository, _mapper) = (repository, mapper);
        public async Task<TransactionTypeDTO> Handle(GetTransactionTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var transactionType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (transactionType == null)
            {
                throw new NotFoundException(nameof(transactionType), request.Id);
            }

            var vm = _mapper.Map<TransactionTypeDTO>(transactionType);
            return vm;
        }
    }
}
