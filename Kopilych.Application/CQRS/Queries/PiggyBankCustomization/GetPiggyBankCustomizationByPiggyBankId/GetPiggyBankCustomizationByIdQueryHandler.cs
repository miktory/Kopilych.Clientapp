using AutoMapper;
using Kopilych.Application.Common.Exceptions;
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PiggyBankCustomization.GetPiggyBankCustomizationByPiggyBankId
{
    public class GetPiggyBankCustomizationByPiggyBankIdQueryHandler : IRequestHandler<GetPiggyBankCustomizationByPiggyBankIdQuery, PiggyBankCustomizationDTO>
    {
        private readonly IPiggyBankCustomizationRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        private readonly IPiggyBankService _piggyBankService;

        public GetPiggyBankCustomizationByPiggyBankIdQueryHandler(IPiggyBankCustomizationRepository repository, IMapper mapper, IUserInfoService userInfoService, IPiggyBankService piggyBankService)
            => (_repository, _mapper, _userInfoService, _piggyBankService) = (repository, mapper, userInfoService, piggyBankService);
        public async Task<PiggyBankCustomizationDTO> Handle(GetPiggyBankCustomizationByPiggyBankIdQuery request, CancellationToken cancellationToken)
        {
            var piggyBankCustomization = (await _repository.GetAllForPiggyBankAsync(request.PiggyBankId, cancellationToken)).FirstOrDefault();
            if (piggyBankCustomization == null)
            {
                throw new NotFoundException(typeof(Domain.PiggyBankCustomization).ToString(), request.PiggyBankId);
            }

            var piggybank = piggyBankCustomization.PiggyBank;
            if (piggybank.OwnerId != request.InitiatorUserId && !request.IsExecuteByAdmin)
            {
                var isMember = false;
                var hasFriendAccess = false;
                // являемся ли мы участником копилки
                try
                {
                    var link = await _piggyBankService.GetUserPiggyBankLinkByUserIdAndPiggyBankId(request.InitiatorUserId, piggybank.Id, cancellationToken);
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
                                break;
                            }
                        }
                        catch (NotFoundException ex)
                        {

                        }
                    }





                }

                if (!isMember && !hasFriendAccess)
                    throw new AccessDeniedException();
            }

            var dto = _mapper.Map<PiggyBankCustomizationDTO>(piggyBankCustomization);
            return dto;
        }
    }
}
