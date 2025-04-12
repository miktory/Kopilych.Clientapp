using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId
{
    public class GetUserDetailsByExternalIdQueryHandler : IRequestHandler<GetUserDetailsByExternalIdQuery, UserDetailsVm>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        public GetUserDetailsByExternalIdQueryHandler(IUserRepository repository, IMapper mapper) => (_repository, _mapper) = (repository, mapper);
        public async Task<UserDetailsVm> Handle(GetUserDetailsByExternalIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByGuidAsync(request.ExternalId, cancellationToken);
            if (user == null || user.ExternalUserGuid != request.ExternalId)
            {
                throw new NotFoundException(nameof(user), request.ExternalId);
            }

            return _mapper.Map<UserDetailsVm>(user);
        }
    }
}
