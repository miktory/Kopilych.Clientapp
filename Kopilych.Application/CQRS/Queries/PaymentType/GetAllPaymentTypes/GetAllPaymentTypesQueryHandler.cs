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

namespace Kopilych.Application.CQRS.Queries.PaymentType.GetAllPaymentTypes
{
    public class GetAllPaymentTypesQueryHandler : IRequestHandler<GetAllPaymentTypesQuery, List<PaymentTypeDTO>>
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPaymentTypesQueryHandler(IPaymentTypeRepository repository, IMapper mapper)
            => (_repository, _mapper) = (repository, mapper);
        public async Task<List<PaymentTypeDTO>> Handle(GetAllPaymentTypesQuery request, CancellationToken cancellationToken)
        {
            var paymentTypes = await _repository.GetAllAsync(cancellationToken);
            if (paymentTypes == null)
            {
                throw new NotFoundException(nameof(paymentTypes), "none");
            }
            
            var result = new List<PaymentTypeDTO>();
            foreach (var pt in paymentTypes)
                result.Add(_mapper.Map<PaymentTypeDTO>(pt));
            return result;
        }
    }
}
