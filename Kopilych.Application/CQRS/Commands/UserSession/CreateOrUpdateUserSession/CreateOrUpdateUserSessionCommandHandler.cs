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

namespace Kopilych.Application.CQRS.Commands.UserSession.CreateOrUpdateUserSession
{
    public class CreateOrUpdateUserSessionCommandHander : IRequestHandler<CreateOrUpdateUserSessionCommand, int>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IUserInfoService _userInfoService;
        public CreateOrUpdateUserSessionCommandHander(IUserSessionRepository repository, IUserInfoService userInfoService)
        {
            _repository = repository;
            _userInfoService = userInfoService;
        }

        public async Task<int> Handle(CreateOrUpdateUserSessionCommand request, CancellationToken cancellationToken)
        {
            // configure max piggy bank links 

            UserDetailsDTO  user = await _userInfoService.GetUserDetailsAsync(request.UserId, cancellationToken, false);
            Domain.UserSession session = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (session == null) 
            {
                session = new Domain.UserSession { AccessToken = request.AccessToken, RefreshToken = request.RefreshToken, UserId = request.UserId };
               await _repository.AddAsync(session, cancellationToken);
            }
            else
            {
                session.AccessToken = request.AccessToken;
                session.RefreshToken = request.RefreshToken;
                session.UserId = request.UserId;
                await _repository.UpdateAsync(session);
            }
            await _repository.SaveChangesAsync(cancellationToken);

            return session.Id;
        }
    }
}
