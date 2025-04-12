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

namespace Kopilych.Application.CQRS.Queries.User.GetUserDetailsById
{
    public class GetUserDetailsByIdQueryHandler : IRequestHandler<GetUserDetailsByIdQuery, UserDetailsVm>
    {
        private readonly IUserRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IMapper _mapper;
        public GetUserDetailsByIdQueryHandler(IUserRepository repository, IMapper mapper, IUserInfoService userInfoService, IPiggyBankService piggyBankService ) => (_repository, _mapper, _userInfoService, _piggyBankService) = (repository, mapper, userInfoService, piggyBankService);
        public async Task<UserDetailsVm> Handle(GetUserDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (user == null || user.Id != request.Id)
            {
                throw new NotFoundException(nameof(user), request.Id);
            }

            if (!request.IsExecuteByAdmin && request.Id != request.InitiatorUserId)
            {
                var isFriends = await _userInfoService.CheckIfApprovedFriendRequestExistsAsync(request.Id, request.InitiatorUserId, cancellationToken);
                var commonPiggyBanksCount = await _piggyBankService.GetCommonPiggyBanksCountForUsersAsync(request.Id, request.InitiatorUserId, cancellationToken);
                if (!isFriends && commonPiggyBanksCount == 0)
                    throw new AccessDeniedException();
            }

            return _mapper.Map<UserDetailsVm>(user);
        }
    }
}
