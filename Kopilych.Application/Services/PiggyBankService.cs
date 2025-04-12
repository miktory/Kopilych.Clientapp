using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.DeleteUser;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.DeleteUserPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetUserPiggyBanksCount;
using Kopilych.Application.CQRS.Queries.PiggyBankType.GetPiggyBankTypeById;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetLinksToCommonPiggyBank;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinkByUserIdAndPiggyBankId;
using Kopilych.Application.CQRS.Queries.UserPiggyBank.GetUserPiggyBankLinksByPiggyBankId;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kopilych.Application.Services
{
    internal class PiggyBankService : IPiggyBankService
    {
        private IMediator _mediator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public PiggyBankService(IServiceScopeFactory serviceScopeFactory) 
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task<PiggyBankVm> GetPiggyBankDetailsAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            var piggybank = await ExecuteWithMediator(async mediator => await mediator.Send(new GetPiggyBankQuery { Id = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            return piggybank;
        }

        public async Task<int> GetCommonPiggyBanksCountForUsersAsync(int userId1, int userId2, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetCommonPiggyBanksIdsForUsersQuery { FirstUserId = userId1, SecondUserId = userId2, IsExecuteByAdmin = true, InitiatorUserId = 0 }, cancellationToken))).Count();
        }


        public async Task<int> GetUserPiggyBankLinksCountForPiggyBank(int piggyBankId, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinksByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true}, cancellationToken))).Count();
        }

        public async Task<int> GetCurrentPiggyBanksCountForUserAsync(int userId, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBanksCountQuery { UserId = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
        }

        public async Task<PiggyBankTypeDTO> GetPiggyBankTypeDetailsAsync(int id, CancellationToken cancellationToken)
        {
            return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPiggyBankTypeByIdQuery { Id = id, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
        }

        public async Task<UserPiggyBankVm> GetUserPiggyBankLinkByUserIdAndPiggyBankId(int userId, int piggyBankId, CancellationToken cancellationToken)
        {
            var link = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinkByUserIdAndPiggyBankIdQuery { PiggyBankId = piggyBankId, UserId = userId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            return link;
        }

        public async Task UnlinkAllUsersExceptOwnerAsync(int piggyBankId, CancellationToken cancellationToken)
        {
            var piggybank = await this.GetPiggyBankDetailsAsync(piggyBankId, cancellationToken);
            var links = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetUserPiggyBankLinksByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            foreach (var link in links) 
            {
                if (link.UserId != piggybank.OwnerId)
                    await ExecuteWithMediator(async mediator => await _mediator.Send(new DeleteUserPiggyBankCommand { Id = link.Id, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
            }
        }

        private async Task<T> ExecuteWithMediator<T>(Func<IMediator, Task<T>> func)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await func(_mediator);
            }
        }

    }
}
