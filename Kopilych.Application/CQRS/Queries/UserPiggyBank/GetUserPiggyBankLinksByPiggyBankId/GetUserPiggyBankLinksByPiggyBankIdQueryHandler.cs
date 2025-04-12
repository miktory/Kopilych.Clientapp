using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Queries.User.GetUserDetailsByExternalId;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByPiggyBankId
{
    public class GetUserPiggyBankLinksByPiggyBankIdQueryHandler : IRequestHandler<GetUserPiggyBankLinksByPiggyBankIdQuery, List<UserPiggyBankVm>>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IUserPiggyBankRepository _upbRepository;
        private readonly IMapper _mapper;
        public GetUserPiggyBankLinksByPiggyBankIdQueryHandler(IPiggyBankRepository repository, IMapper mapper, IUserPiggyBankRepository upbRepository)
            => (_repository, _mapper, _upbRepository) = (repository, mapper, upbRepository);
        public async Task<List<UserPiggyBankVm>> Handle(GetUserPiggyBankLinksByPiggyBankIdQuery request, CancellationToken cancellationToken)
        {
            var userPiggyBankLinks = await _upbRepository.GetAllForPiggyBankAsync(request.PiggyBankId, cancellationToken);
            var result = new List<UserPiggyBankVm>();
            if (!request.IsExecuteByAdmin && userPiggyBankLinks.Where(x => x.UserId == request.InitiatorUserId).Count() == 0)
                throw new AccessDeniedException();
            foreach (var l in userPiggyBankLinks)
            {
                var vm = _mapper.Map<UserPiggyBankVm>(l);
                result.Add(vm);
            }
             
            return result;
        }
    }
}
