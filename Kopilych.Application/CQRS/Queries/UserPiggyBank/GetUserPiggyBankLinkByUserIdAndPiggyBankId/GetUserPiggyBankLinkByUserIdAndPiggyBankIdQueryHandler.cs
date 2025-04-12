using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
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

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinkByUserIdAndPiggyBankId
{
    public class GetUserPiggyBankLinkByUserIdAndPiggyBankIdQueryHandler : IRequestHandler<GetUserPiggyBankLinkByUserIdAndPiggyBankIdQuery, UserPiggyBankVm>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IUserPiggyBankRepository _upbRepository;
        private readonly IMapper _mapper;
        private readonly IUserInfoService _userInfoService;
        public GetUserPiggyBankLinkByUserIdAndPiggyBankIdQueryHandler(IPiggyBankRepository repository, IMapper mapper, IUserPiggyBankRepository upbRepository, IUserInfoService userInfoService)
            => (_repository, _mapper, _upbRepository, _userInfoService) = (repository, mapper, upbRepository, userInfoService);
        public async Task<UserPiggyBankVm> Handle(GetUserPiggyBankLinkByUserIdAndPiggyBankIdQuery request, CancellationToken cancellationToken)
        {
            var userPiggyBank = await _upbRepository.GetByUserIdAndPiggyBankIdAsync(request.UserId, request.PiggyBankId, cancellationToken);
            var pbOwner = userPiggyBank == null ? false : userPiggyBank.PiggyBank.OwnerId == request.InitiatorUserId;
            if (!request.IsExecuteByAdmin && request.InitiatorUserId != userPiggyBank.UserId)
            {
                var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(request.UserId, request.InitiatorUserId, cancellationToken);

                if (!isFriends && !pbOwner)
                    throw new AccessDeniedException();
            }

            if (userPiggyBank == null)
                throw new NotFoundException(nameof(userPiggyBank), $"{request.UserId} {request.PiggyBankId}");

            if (!pbOwner && !userPiggyBank.Public && !request.IsExecuteByAdmin && request.InitiatorUserId != userPiggyBank.UserId)
                throw new NotFoundException(nameof(userPiggyBank), $"{request.UserId} {request.PiggyBankId}"); // вместо 403 выбросим 404, чтобы было непонятно: существует ресурс или нет

            var result = _mapper.Map<UserPiggyBankVm>(userPiggyBank);
             
            return result;
        }
    }
}
