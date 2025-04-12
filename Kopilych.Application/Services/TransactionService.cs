using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.User.DeleteUser;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.DeleteUserPiggyBank;
using Kopilych.Application.CQRS.Queries.PaymentType.GetPaymentTypeById;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetUserPiggyBanksCount;
using Kopilych.Application.CQRS.Queries.PiggyBankType.GetPiggyBankTypeById;
using Kopilych.Application.CQRS.Queries.TransactionType.GetTransactionTypeById;
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
    internal class TransactionService : ITransactionService
    {
        private IMediator _mediator;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public TransactionService(IServiceScopeFactory serviceScopeFactory) 
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        private async Task<T> ExecuteWithMediator<T>(Func<IMediator, Task<T>> func)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                return await func(_mediator);
            }
        }

        public async Task<TransactionTypeDTO> GetTransactionTypeDetailsAsync(int transactionTypeId, CancellationToken cancellationToken)
        {
            var tt = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetTransactionTypeByIdQuery { Id = transactionTypeId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            return tt;
        }

        public async Task<PaymentTypeDTO> GetPaymentTypeDetailsAsync(int paymentTypeId, CancellationToken cancellationToken)
        {
            var pt = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetPaymentTypeByIdQuery { Id = paymentTypeId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            return pt;
        }
    }
}
