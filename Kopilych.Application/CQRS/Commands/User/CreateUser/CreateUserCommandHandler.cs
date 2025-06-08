using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.Common.Exceptions;

namespace Kopilych.Application.CQRS.Commands.User.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
    {
        private readonly IUserRepository _repository;
        public CreateUserCommandHandler(IUserRepository repository) => _repository = repository;
        public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsExecuteByAdmin)
                throw new AccessDeniedException();
            var user = new Domain.User
            {
                Username = request.Username,
                ExternalId = request.ExternalId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                PhotoPath = request.PhotoPath,
                Version = 0,
                PhotoIntegrated = request.PhotoIntegrated,
            };

            await _repository.AddAsync(user, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
