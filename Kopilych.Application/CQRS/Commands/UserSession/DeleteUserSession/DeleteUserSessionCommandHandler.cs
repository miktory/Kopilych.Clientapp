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

namespace Kopilych.Application.CQRS.Commands.UserSession.DeleteUserSession
{
    public class DeleteUserSessionCommandHander : IRequestHandler<DeleteUserSessionCommand, Unit>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IUserInfoService _userInfoService;
        public DeleteUserSessionCommandHander(IUserSessionRepository repository, IUserInfoService userInfoService)
        {
            _repository = repository;
            _userInfoService = userInfoService;
        }

        public async Task<Unit> Handle(DeleteUserSessionCommand request, CancellationToken cancellationToken)
        {
            Domain.UserSession session = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (session == null) 
            {
                throw new NotFoundException(typeof(Domain.UserSession).ToString(), $"{request.UserId}");
            }

            await _repository.DeleteAsync(session);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
