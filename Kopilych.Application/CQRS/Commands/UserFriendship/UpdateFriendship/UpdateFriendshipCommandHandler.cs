using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Common.Exceptions;

namespace Kopilych.Application.CQRS.Commands.UserFriendship.UpdateFriendship
{
    public class UpdateFriendshipCommandHandler : IRequestHandler<UpdateFriendshipCommand>
    {
        private readonly IUserFriendshipRepository _repository;
        public UpdateFriendshipCommandHandler(IUserFriendshipRepository repository) => _repository = repository;
        public async Task Handle(UpdateFriendshipCommand request, CancellationToken cancellationToken)
        {
            var friendship = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (friendship == null)
            {
                throw new NotFoundException(nameof(friendship), request.RequestId);
            }
            else if (friendship.ApproverUserId != request.InitiatorUserId && !request.IsExecuteByAdmin)
                throw new AccessDeniedException();
            if (friendship.RequestApproved == request.RequestApproved)
                return;
            friendship.RequestApproved = request.RequestApproved;
            friendship.Updated = DateTime.UtcNow;
            await _repository.SaveChangesAsync(cancellationToken);
            return;
        }
    }
}
