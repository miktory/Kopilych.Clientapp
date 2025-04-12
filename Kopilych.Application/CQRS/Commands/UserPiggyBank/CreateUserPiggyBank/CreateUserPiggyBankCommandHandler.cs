using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByPiggyBankId;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using Kopilych.Shared;
using Kopilych.Application.Interfaces;

namespace Kopilych.Application.CQRS.Commands.UserPiggyBank.CreateUserPiggyBank
{
    public class CreateUserPiggyBankCommandHandler : IRequestHandler<CreateUserPiggyBankCommand, int>
    {
        private readonly IUserPiggyBankRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public CreateUserPiggyBankCommandHandler(IUserPiggyBankRepository repository, IUserInfoService userInfoService, IPiggyBankService piggyBankService, IUserRestrictionsSettings userRestrictionsSettings)
        {
            _repository = repository;
            _userInfoService = userInfoService;
            _piggyBankService = piggyBankService;
            _userRestrictionsSettings = userRestrictionsSettings;
        }

        public async Task<int> Handle(CreateUserPiggyBankCommand request, CancellationToken cancellationToken)
        {
            // configure max piggy bank links 

            UserDetailsVm invitableUser = await _userInfoService.GetUserDetailsAsync(request.UserId, cancellationToken);
            PiggyBankVm piggyBank = await _piggyBankService.GetPiggyBankDetailsAsync(request.PiggyBankId, cancellationToken);


            if (!request.IsExecuteByAdmin)
            {
                UserDetailsVm user = await _userInfoService.GetUserDetailsAsync(request.InitiatorUserId, cancellationToken);
                if (user.Id != piggyBank.OwnerId)
                    throw new AccessDeniedException();
                if (user.Id != request.UserId)
                {
                    if (!piggyBank.Shared)
                        throw new AccessDeniedException();
                    var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(user.Id, invitableUser.Id, cancellationToken);
                    if (!isFriends)
                        throw new AccessDeniedException();
                }
            }
            var linkscount = await _piggyBankService.GetUserPiggyBankLinksCountForPiggyBank(piggyBank.Id, cancellationToken);
            if (!request.IsExecuteByAdmin && linkscount >= _userRestrictionsSettings.MaxLinksPerPiggyBankCount)
                throw new AccessDeniedException();

            var upb = await _repository.GetByUserIdAndPiggyBankIdAsync(request.UserId, request.PiggyBankId, cancellationToken);
            if (upb != null)
                throw new AlreadyExistsException();
            upb = new Domain.UserPiggyBank
            {
                PiggyBankId = request.PiggyBankId,
                UserId = request.UserId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                HideBalance = request.HideBalance,
                Public = request.Public,
                Version = 0
            };

            await _repository.AddAsync(upb, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return upb.Id;
        }
    }
}
