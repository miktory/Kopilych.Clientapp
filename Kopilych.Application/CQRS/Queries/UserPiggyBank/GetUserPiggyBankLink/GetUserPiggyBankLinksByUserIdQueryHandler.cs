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

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLink
{
    public class GetUserPiggyBankLinkQueryHandler : IRequestHandler<GetUserPiggyBankLinkQuery, UserPiggyBankVm>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IUserPiggyBankRepository _upbRepository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        public GetUserPiggyBankLinkQueryHandler(IPiggyBankRepository repository, IMapper mapper, IUserPiggyBankRepository upbRepository, IUserInfoService userInfoService)
            => (_repository, _mapper, _upbRepository, _userInfoService) = (repository, mapper, upbRepository, userInfoService);
        public async Task<UserPiggyBankVm> Handle(GetUserPiggyBankLinkQuery request, CancellationToken cancellationToken)
        {
            var userPiggyBank = await _upbRepository.GetByIdAsync(request.Id, cancellationToken);
            var pbOwner = userPiggyBank == null ? false : userPiggyBank.PiggyBank.OwnerId == request.InitiatorUserId;

            if (!request.IsExecuteByAdmin && request.InitiatorUserId != userPiggyBank.UserId)
            {
                var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(request.InitiatorUserId, userPiggyBank.UserId, cancellationToken);
    
                if (!isFriends && !pbOwner)
                    throw new AccessDeniedException();
            }
            if (userPiggyBank == null)
                throw new NotFoundException(nameof(userPiggyBank), $"{request.Id}");

            if (!pbOwner && !userPiggyBank.Public && !request.IsExecuteByAdmin && request.InitiatorUserId != userPiggyBank.UserId)
                throw new NotFoundException(nameof(userPiggyBank), $"{request.Id}"); // вместо 403 выбросим 404, чтобы было непонятно: существует ресурс или нет


            var vm = _mapper.Map<UserPiggyBankVm>(userPiggyBank);
             
            return vm;
        }
    }
}
