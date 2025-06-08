using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByUserId
{
    public class GetUserPiggyBankLinksByUserIdQueryHandler : IRequestHandler<GetUserPiggyBankLinksByUserIdQuery, List<UserPiggyBankDTO>>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IUserPiggyBankRepository _upbRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        public GetUserPiggyBankLinksByUserIdQueryHandler(IPiggyBankRepository repository, IMapper mapper, IUserPiggyBankRepository upbRepository, IUserInfoService userInfoService)
            => (_repository, _mapper, _upbRepository, _userInfoService) = (repository, mapper, upbRepository, userInfoService);
        public async Task<List<UserPiggyBankDTO>> Handle(GetUserPiggyBankLinksByUserIdQuery request, CancellationToken cancellationToken)
        {
            var userPiggyBankLinks = await _upbRepository.GetAllForUserAsync(request.UserId, cancellationToken);
            var result = new List<UserPiggyBankDTO>();

            if (!request.IsExecuteByAdmin && request.InitiatorUserId != request.UserId)
            {
                var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(request.UserId, request.InitiatorUserId, cancellationToken);
                if (!isFriends)
                    throw new AccessDeniedException();
            }

            foreach (var l in userPiggyBankLinks)
            {
                var vm = _mapper.Map<UserPiggyBankDTO>(l);
                if (!request.IsExecuteByAdmin)
                {
                    if (!l.Public && request.InitiatorUserId != request.UserId)
                        continue;
                }
                result.Add(vm);
            }
             
            return result;
        }
    }
}
