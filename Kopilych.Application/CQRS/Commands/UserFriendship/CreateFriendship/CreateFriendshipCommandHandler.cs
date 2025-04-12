using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsById;
using Kopilych.Application.Interfaces;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.CreateFriendship
{
    public class CreateFriendshipCommandHandler : IRequestHandler<CreateFriendshipCommand, int>
    {
        private readonly IUserFriendshipRepository _repository;
        private readonly IUserInfoService _userInfoService;

        public CreateFriendshipCommandHandler(IUserFriendshipRepository repository, IUserInfoService userInfoService)
        {
            _repository = repository;
            _userInfoService = userInfoService;
        }

        public async Task<int> Handle(CreateFriendshipCommand request, CancellationToken cancellationToken)
        {
            var friendship = await _repository.GetBySpecifiedUserIdsAsync(request.InitiatorUserId, request.ApproverUserId, cancellationToken);
            if (friendship != null)
                throw new AlreadyExistsException();
            if (request.InitiatorUserId == request.ApproverUserId)
                throw new AlreadyExistsException();
            var firstUser = await _userInfoService.GetUserDetailsAsync(request.InitiatorUserId, cancellationToken);
            var secondUser = await _userInfoService.GetUserDetailsAsync(request.ApproverUserId, cancellationToken);
            friendship = new Domain.UserFriendship
            {
                InitiatorUserId = request.InitiatorUserId,
                ApproverUserId = request.ApproverUserId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                RequestApproved = false
            };

            await _repository.AddAsync(friendship, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return friendship.Id;
        }
    }
}
