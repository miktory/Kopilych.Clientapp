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

namespace Kopilych.Application.CQRS.Commands.UserPiggyBank.DeleteUserPiggyBank
{
    public class DeleteUserPiggyBankCommandHandler : IRequestHandler<DeleteUserPiggyBankCommand, Unit>
    {
        private readonly IUserPiggyBankRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public DeleteUserPiggyBankCommandHandler(IUserPiggyBankRepository repository, IUserInfoService userInfoService, IPiggyBankService piggyBankService, IUserRestrictionsSettings userRestrictionsSettings)
        {
            _repository = repository;
            _userInfoService = userInfoService;
            _piggyBankService = piggyBankService;
            _userRestrictionsSettings = userRestrictionsSettings;
        }

        public async Task<Unit> Handle(DeleteUserPiggyBankCommand request, CancellationToken cancellationToken)
        {

            var userPiggyBank = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (userPiggyBank == null)
                throw new NotFoundException(nameof(userPiggyBank), $"{request.Id}");
            Domain.PiggyBank piggyBank = userPiggyBank.PiggyBank;

            if (!request.IsExecuteByAdmin)
            {
                UserDetailsVm user = await _userInfoService.GetUserDetailsAsync(request.InitiatorUserId, cancellationToken);
                if (user.Id != piggyBank.OwnerId && userPiggyBank.UserId != user.Id)
                    throw new AccessDeniedException();
            }
            var id = userPiggyBank.Id;
            await _repository.DeleteAsync(userPiggyBank);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
