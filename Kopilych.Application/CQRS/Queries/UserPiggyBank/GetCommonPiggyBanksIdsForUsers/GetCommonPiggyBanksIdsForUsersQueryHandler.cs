using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetLinksToCommonPiggyBank;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetCommonPiggyBanksIdsForUsers
{
    public class GetCommonPiggyBanksIdsForUsersQueryHandler : IRequestHandler<GetCommonPiggyBanksIdsForUsersQuery, List<int>>
    {
        private readonly IUserPiggyBankRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        public GetCommonPiggyBanksIdsForUsersQueryHandler(IMapper mapper, IUserPiggyBankRepository repository, IUserInfoService userInfoService)
            => (_repository, _mapper, _userInfoService) = (repository, mapper, userInfoService);
        public async Task<List<int>> Handle(GetCommonPiggyBanksIdsForUsersQuery request, CancellationToken cancellationToken)
        {
            var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(request.FirstUserId, request.SecondUserId, cancellationToken);
            var result = (await _repository.GetCommonPiggyBankIdsForUsersAsync(request.FirstUserId, request.SecondUserId, cancellationToken)).ToList();
            if (!isFriends && request.InitiatorUserId != request.FirstUserId && request.InitiatorUserId != request.SecondUserId)
                if (result.Count == 0)
                    if (!request.IsExecuteByAdmin)
                        throw new AccessDeniedException();

            return result.ToList();
        }
    }
}
