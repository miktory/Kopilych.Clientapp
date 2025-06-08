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

namespace Kopilych.Application.CQRS.Queries.UserFriendship.GetFriendshipDetailsById
{
    public class GetFriendshipDetailsByIdQueryHandler : IRequestHandler<GetFriendshipDetailsByIdQuery, UserFriendshipDetailsDTO>
    {
        private readonly IUserFriendshipRepository _repository;
        private readonly IMapper _mapper;
        public GetFriendshipDetailsByIdQueryHandler(IUserFriendshipRepository repository, IMapper mapper) => (_repository, _mapper) = (repository, mapper);
        public async Task<UserFriendshipDetailsDTO> Handle(GetFriendshipDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var friendship = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (friendship == null)
            {
                throw new NotFoundException(nameof(friendship), $"{request.Id}");
            }
            if (!request.IsExecuteByAdmin && friendship.InitiatorUserId != request.InitiatorUserId && friendship.ApproverUserId != request.InitiatorUserId)
                throw new AccessDeniedException();
            var result = _mapper.Map<UserFriendshipDetailsDTO>(friendship);

            return result;
        }
    }
}
