using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.UpdateUser;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.User.GetCurrentUserDetails
{
    public class GetCurrentUserDetailsQueryHandler : IRequestHandler<GetCurrentUserDetailsQuery, UserDetailsDTO>
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        public GetCurrentUserDetailsQueryHandler(IUserRepository repository, IMapper mapper, IUserInfoService userInfoService, IPiggyBankService piggyBankService ) => (_repository, _mapper) = (repository, mapper);
        public async Task<UserDetailsDTO> Handle(GetCurrentUserDetailsQuery request, CancellationToken cancellationToken)
        {
            var user = (await _repository.GetAllAsync(cancellationToken)).FirstOrDefault();
            if (user == null)
            {
                throw new NotFoundException(nameof(user), null);
            }

            return _mapper.Map<UserDetailsDTO>(user);
        }
    }
}
