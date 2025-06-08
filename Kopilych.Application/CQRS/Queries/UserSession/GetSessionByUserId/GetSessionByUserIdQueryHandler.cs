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
using AutoMapper;

namespace Kopilych.Application.CQRS.Queries.UserSession.GetSessionByUserId
{
    public class GetSessionByUserIdQueryHander : IRequestHandler<GetSessionByUserIdQuery, UserSessionDTO>
    {
        private readonly IUserSessionRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IMapper _mapper;
        public GetSessionByUserIdQueryHander(IUserSessionRepository repository, IUserInfoService userInfoService, IMapper mapper)
        {
            _repository = repository;
            _userInfoService = userInfoService;
            _mapper = mapper;
        }

        public async Task<UserSessionDTO> Handle(GetSessionByUserIdQuery request, CancellationToken cancellationToken)
        {
            Domain.UserSession session = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
            if (session == null) 
            {
                throw new NotFoundException(typeof(Domain.UserSession).ToString(), $"{request.UserId}");
            }


            return _mapper.Map<UserSessionDTO>(session);
        }
    }
}
