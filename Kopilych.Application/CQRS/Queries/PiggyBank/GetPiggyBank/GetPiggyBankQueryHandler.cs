using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByUserId;
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

namespace Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank
{
    public class GetPiggyBankQueryHandler : IRequestHandler<GetPiggyBankQuery, PiggyBankDTO>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        private readonly IPiggyBankService _piggyBankService;

        public GetPiggyBankQueryHandler(IPiggyBankRepository repository, IMapper mapper, IUserInfoService userInfoService, IPiggyBankService piggyBankService)
            => (_repository, _mapper, _userInfoService, _piggyBankService) = (repository, mapper, userInfoService, piggyBankService);
        public async Task<PiggyBankDTO> Handle(GetPiggyBankQuery request, CancellationToken cancellationToken)
        {
            var piggybank = await _repository.GetByIdAsync(request.Id, cancellationToken);
            var hideInfo = true;
            if (piggybank == null)
            {
                throw new NotFoundException(nameof(piggybank), request.Id);
            }

            if (piggybank.OwnerId != request.InitiatorUserId && !request.IsExecuteByAdmin)
            {
                var isMember = false;
                var hasFriendAccess = false;
                // являемся ли мы участником копилки
                try
                {
                    var link = await _piggyBankService.GetUserPiggyBankLinkByUserIdAndPiggyBankId(request.InitiatorUserId, request.Id, cancellationToken);
                    isMember = true;
                }
                catch (NotFoundException ex)
                {

                }
                if (!isMember)
                {
                    if (piggybank.Shared)
                        throw new AccessDeniedException();

                    var friendRequests = await _userInfoService.GetAllUserFriendshipDetailsAsync(request.InitiatorUserId, cancellationToken, false);
                    friendRequests = friendRequests.Where(x => x.RequestApproved).ToList();

                    foreach (var f in friendRequests)
                    {
                        try
                        {
                            var friendId = f.InitiatorUserId == request.InitiatorUserId ? f.ApproverUserId : f.InitiatorUserId;
                            var link = await _piggyBankService.GetUserPiggyBankLinkByUserIdAndPiggyBankId(friendId, piggybank.Id, cancellationToken);
                            if (link != null && link.Public)
                            {
                                hasFriendAccess = true;
                                if (!link.HideBalance)
                                {
                                    hideInfo = false;
                                    break;
                                }
                            }
                        }
                        catch (NotFoundException ex)
                        {

                        }
                    }





                }

                if (!isMember && !hasFriendAccess)
                    throw new AccessDeniedException();
                if (isMember)
                    hideInfo = false;
            }

            var vm = _mapper.Map<PiggyBankDTO>(piggybank);
            if (hideInfo && !request.IsExecuteByAdmin && piggybank.OwnerId != request.InitiatorUserId)
            {
                vm.Balance = 0;
                vm.Goal = 0;

            }
            return vm;
        }
    }
}
