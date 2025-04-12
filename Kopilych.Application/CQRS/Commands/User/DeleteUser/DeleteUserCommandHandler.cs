using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.User.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Unit>
    {
        private readonly IUserRepository _repository;
        public DeleteUserCommandHandler(IUserRepository repository) => _repository = repository;
        public async Task<Unit> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null || user.Id != request.Id)
            {
                throw new NotFoundException(nameof(user), request.Id);
            }

            if (request.InitiatorUserId != user.Id && !request.IsExecuteByAdmin)
                throw new AccessDeniedException();

            await _repository.DeleteAsync(user);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
