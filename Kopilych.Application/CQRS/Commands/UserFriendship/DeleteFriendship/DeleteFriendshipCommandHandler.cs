using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Common.Exceptions;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.DeleteFriendship
{
    public class DeleteFriendshipCommandHandler : IRequestHandler<DeleteFriendshipCommand>
    {
        private readonly IUserFriendshipRepository _repository;
        public DeleteFriendshipCommandHandler(IUserFriendshipRepository repository) => _repository = repository;
        public async Task Handle(DeleteFriendshipCommand request, CancellationToken cancellationToken)
        {
            var friendship = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (friendship == null)
                throw new NotFoundException(nameof(friendship), request.Id);

            if (!request.IsExecutedByAdmin && friendship.ApproverUserId != request.InitiatorUserId && friendship.InitiatorUserId != request.InitiatorUserId)
                throw new AccessDeniedException();

            await _repository.DeleteAsync(friendship);
            await _repository.SaveChangesAsync(cancellationToken);

            return;
        }
    }
}
