using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus
{
    public class GetUserPremiumStatusQueryHandler : IRequestHandler<GetUserPremiumStatusQuery, PremiumStatusVm>
    {
        private readonly IPremiumUserRepository _repository;
        private readonly IMapper _mapper;
        public GetUserPremiumStatusQueryHandler(IPremiumUserRepository repository, IMapper mapper) => (_repository, _mapper) = (repository, mapper);
        public async Task<PremiumStatusVm> Handle(GetUserPremiumStatusQuery request, CancellationToken cancellationToken)
        {
            var premium = await _repository.GetForUserAsync(request.UserId, cancellationToken);
            var result = new PremiumStatusVm { Premium = false, ExpirationDate = null };
            if (premium != null)
            {
                result.Premium = true;
                result.ExpirationDate = premium.ExpirationDate;
            }

            return result;
        }
    }
}
