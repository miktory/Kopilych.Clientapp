using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;

namespace Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsByUserIds
{
    public class GetFriendshipDetailsByUserIdsQueryHandler : IRequestHandler<GetFriendshipDetailsByUserIdsQuery, UserFriendshipDetailsDTO>
    {
        private readonly IUserFriendshipRepository _repository;
        private readonly IMapper _mapper;
        public GetFriendshipDetailsByUserIdsQueryHandler(IUserFriendshipRepository repository, IMapper mapper) => (_repository, _mapper) = (repository, mapper);
        public async Task<UserFriendshipDetailsDTO> Handle(GetFriendshipDetailsByUserIdsQuery request, CancellationToken cancellationToken)
        {
            var friendship = await _repository.GetBySpecifiedUserIdsAsync(request.FirstUserId, request.SecondUserId, cancellationToken);
            if (friendship == null)
            {
                throw new NotFoundException(nameof(friendship), $"{request.FirstUserId} {request.SecondUserId}");
            }
            else if (friendship.InitiatorUserId != request.InitiatorUserId && friendship.ApproverUserId != request.InitiatorUserId && !request.IsExecuteByAdmin)
                throw new AccessDeniedException();

            var result = _mapper.Map<UserFriendshipDetailsDTO>(friendship);

            return result;
        }
    }
}
