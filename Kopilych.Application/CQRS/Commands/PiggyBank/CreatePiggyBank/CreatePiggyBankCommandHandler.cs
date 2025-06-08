using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Domain;
using Kopilych.Application.Interfaces.Repository;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetUserPiggyBanksCount;
using Kopilych.Application.CQRS.Queries.PremiumUser.GetUserPremiumStatus;
using Kopilych.Application.Interfaces;
using Kopilych.Shared.DTO;
using Kopilych.Application.Common.Exceptions;

namespace Kopilych.Application.CQRS.Commands.PiggyBank.CreatePiggyBank
{
    public class CreatePiggyBankCommandHandler : IRequestHandler<CreatePiggyBankCommand, int>
    {
        private readonly IPiggyBankRepository _repository;
        private readonly IUserInfoService _userInfoService;
        private readonly IPiggyBankService _piggyBankService;
        private readonly IUserRestrictionsSettings _userRestrictionsSettings;
        public CreatePiggyBankCommandHandler(IPiggyBankRepository repository, IUserInfoService userInfoService, IPiggyBankService piggyBankService, IUserRestrictionsSettings userRestrictionsSettings)
        {
            _repository = repository;
            _userInfoService = userInfoService;
            _piggyBankService = piggyBankService;
            _userRestrictionsSettings = userRestrictionsSettings;

        }
        public async Task<int> Handle(CreatePiggyBankCommand request, CancellationToken cancellationToken)
        {
          
            if (!request.IsExecuteByAdmin)
            {
                if (request.OwnerId != request.InitiatorUserId)
                    throw new AccessDeniedException();


                var premium = await _userInfoService.CheckIfUserPremiumAsync(request.OwnerId, cancellationToken);
                var count = await _piggyBankService.GetCurrentPiggyBanksCountForUserAsync(request.OwnerId, cancellationToken);

                if (!premium)
                {
                    if (count >= _userRestrictionsSettings.MaxPiggyBanksCountWithoutPremium)
                        throw new AccessDeniedException();
                }
                else
                {
                    if (count >= _userRestrictionsSettings.MaxPiggyBanksCountWithPremium)
                        throw new AccessDeniedException();
                }
            }
            var user = await _userInfoService.GetUserDetailsAsync(request.OwnerId, cancellationToken, false);
    
            var piggybank = new Domain.PiggyBank
            {
                Name = request.Name,
                Description = request.Description,
                Balance = request.Balance,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
                Goal = request.Goal,
                GoalDate = request.GoalDate,
                Shared = request.Shared,
                OwnerId = request.OwnerId,
                Version = request.Version,
                ExternalId = request.ExternalId
            };

            await _repository.AddAsync(piggybank, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return piggybank.Id;
        }
    }
}
