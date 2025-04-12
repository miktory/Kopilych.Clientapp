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

namespace Kopilych.Application.CQRS.Queries.PiggyBankType.GetAllPiggyBankTypes
{
    public class GetAllPiggyBankTypesQueryHandler : IRequestHandler<GetAllPiggyBankTypesQuery, List<PiggyBankTypeDTO>>
    {
        private readonly IPiggyBankTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPiggyBankTypesQueryHandler(IPiggyBankTypeRepository repository, IMapper mapper)
            => (_repository, _mapper) = (repository, mapper);
        public async Task<List<PiggyBankTypeDTO>> Handle(GetAllPiggyBankTypesQuery request, CancellationToken cancellationToken)
        {
            var piggyBankTypes = await _repository.GetAllAsync(cancellationToken);
            if (piggyBankTypes == null)
            {
                throw new NotFoundException(nameof(piggyBankTypes), "none");
            }
            
            var result = new List<PiggyBankTypeDTO>();
            foreach (var pbt in piggyBankTypes)
                result.Add(_mapper.Map<PiggyBankTypeDTO>(pbt));
            return result;
        }
    }
}
