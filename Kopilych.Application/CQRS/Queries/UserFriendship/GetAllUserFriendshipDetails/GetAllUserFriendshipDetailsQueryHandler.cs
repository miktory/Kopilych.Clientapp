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

namespace Kopilych.Application.CQRS.Queries.UserFriendship.GetAllUserFriendshipDetails
{
    public class GetAllUserFriendshipDetailsQueryHandler : IRequestHandler<GetAllUserFriendshipDetailsQuery, List<UserFriendshipDetailsVm>>
    {
        private readonly IUserFriendshipRepository _repository;
        private readonly IMapper _mapper;
        public GetAllUserFriendshipDetailsQueryHandler(IUserFriendshipRepository repository, IMapper mapper) => (_repository, _mapper) = (repository, mapper);
        public async Task<List<UserFriendshipDetailsVm>> Handle(GetAllUserFriendshipDetailsQuery request, CancellationToken cancellationToken)
        {
            var friendships = await _repository.GetAllForUserAsync(request.UserId, cancellationToken);
            var result  = new List<UserFriendshipDetailsVm>();
            if (request.UserId != request.InitiatorUserId && !request.IsExecuteByAdmin)
                throw new AccessDeniedException();
            foreach (var f in friendships)
                result.Add(_mapper.Map<UserFriendshipDetailsVm>(f));
            return result;
        }
    }
}
