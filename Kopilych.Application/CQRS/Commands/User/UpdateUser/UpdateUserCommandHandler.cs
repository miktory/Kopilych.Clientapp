using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.User.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
    {
        private readonly IUserRepository _repository;
        public UpdateUserCommandHandler(IUserRepository repository) => _repository = repository;
        public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null || user.Id != request.Id)
            {
                throw new NotFoundException(nameof(user), request.Id);
            }

            if (request.InitiatorUserId != request.Id && !request.IsExecuteByAdmin)
                throw new AccessDeniedException();

            user.Username = request.Username;
            user.Version = request.Version;
            user.PhotoPath = request.PhotoPath;
            user.Updated = DateTime.UtcNow;
            user.ExternalId = request.ExternalId;
            user.PhotoIntegrated = request.PhotoIntegrated;

            await _repository.UpdateAsync(user);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
