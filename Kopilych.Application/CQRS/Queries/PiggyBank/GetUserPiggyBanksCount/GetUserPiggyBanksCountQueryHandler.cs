using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Domain;
using Kopilych.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PiggyBank.GetUserPiggyBanksCount
{
    public class GetUserPiggyBanksCountQueryHandler : IRequestHandler<GetUserPiggyBanksCountQuery, int>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        private readonly IPiggyBankService _piggyBankService;
        public GetUserPiggyBanksCountQueryHandler(IPiggyBankRepository repository, IMapper mapper, IUserInfoService userInfoService, IPiggyBankService piggyBankService)
            => (_repository, _mapper, _userInfoService, _piggyBankService) = (repository, mapper, userInfoService, piggyBankService);
        public async Task<int> Handle(GetUserPiggyBanksCountQuery request, CancellationToken cancellationToken)
        {
            if (!request.IsExecuteByAdmin && request.InitiatorUserId != request.UserId)
            {
                var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(request.InitiatorUserId, request.UserId, cancellationToken);
                if (!isFriends)    
                    throw new AccessDeniedException();
            }
            var piggybanks = await _repository.GetAllForUserAsync(request.UserId, cancellationToken);
            if (piggybanks == null)
            {
                return 0;
            }

            return piggybanks.Count();
        }
    }
}
