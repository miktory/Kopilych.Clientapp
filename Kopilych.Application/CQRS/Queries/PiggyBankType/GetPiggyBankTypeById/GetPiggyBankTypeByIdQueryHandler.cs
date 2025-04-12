using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PaymentType.GetPaymentTypeById;
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

namespace Kopilych.Application.CQRS.Queries.PiggyBankType.GetPiggyBankTypeById
{
    public class GetPiggyBankTypeByIdQueryHandler : IRequestHandler<GetPiggyBankTypeByIdQuery, PiggyBankTypeDTO>
    {
        private readonly IPiggyBankTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetPiggyBankTypeByIdQueryHandler(IPiggyBankTypeRepository repository, IMapper mapper)
            => (_repository, _mapper) = (repository, mapper);
        public async Task<PiggyBankTypeDTO> Handle(GetPiggyBankTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var piggyBankType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (piggyBankType == null)
            {
                throw new NotFoundException(nameof(piggyBankType), request.Id);
            }

            var vm = _mapper.Map<PiggyBankTypeDTO>(piggyBankType);
            return vm;
        }
    }
}
