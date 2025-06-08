using AutoMapper;
using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank;
using Kopilych.Application.CQRS.Commands.Transaction.CreateTransaction;
using Kopilych.Application.CQRS.Commands.Transaction.DeleteTransaction;
using Kopilych.Application.CQRS.Commands.Transaction.UpdateTransaction;
using Kopilych.Application.CQRS.Commands.User.DeleteUser;
using Kopilych.Application.CQRS.Commands.UserPiggyBank.DeleteUserPiggyBank;
using Kopilych.Application.CQRS.Queries.PaymentType.GetAllPaymentTypes;
using Kopilych.Application.CQRS.Queries.PaymentType.GetPaymentTypeById;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetPiggyBank;
using Kopilych.Application.CQRS.Queries.PiggyBank.GetUserPiggyBanksCount;
using Kopilych.Application.CQRS.Queries.PiggyBankType.GetPiggyBankTypeById;
using Kopilych.Application.CQRS.Queries.Transaction.GetAllTransactionsByPiggyBankId;
using Kopilych.Application.CQRS.Queries.Transaction.GetTransactionById;
using Kopilych.Application.CQRS.Queries.TransactionType.GetAllTransactionTypes;
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
using System.Transactions;

namespace Kopilych.Application.Services
{
    internal class TransactionService : ITransactionService
    {
        private IMediator _mediator;
        private IIntegrationService _integrationService;
        private IMapper _mapper;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public TransactionService(IServiceScopeFactory serviceScopeFactory, IMapper mapper, IIntegrationService integrationService) 
        {
            _serviceScopeFactory = serviceScopeFactory;
            _integrationService = integrationService;
            _mapper = mapper;
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

        public async Task<List<PaymentTypeDTO>> GetAllPaymentTypesAsync(CancellationToken cancellationToken)
        {
            var pt = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetAllPaymentTypesQuery { InitiatorUserId = 0, IsExecuteByAdmin = true, }, cancellationToken)));
            return pt;
        }

        public async Task<List<TransactionTypeDTO>> GetAllTransactionTypesAsync(CancellationToken cancellationToken)
        {
            var tt = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetAllTransactionTypesQuery { InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            return tt;
        }

        public async Task<TransactionDTO> GetTransactionDetailsAsync(int transactionId, CancellationToken cancellationToken)
        {
            var t = (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetTransactionByIdQuery { Id = transactionId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            return t;
        }

        public async Task<List<TransactionDTO>> GetTransactionsByPiggyBankIdAsync(int piggyBankId, CancellationToken cancellationToken, bool remote)
        {
            if (!remote)
                return (await ExecuteWithMediator(async mediator => await _mediator.Send(new GetAllTransactionsByPiggyBankIdQuery { PiggyBankId = piggyBankId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken)));
            else
            {
                var list =  await _integrationService.GetTransactionsByPiggyBankIdFromServerAsync(piggyBankId, cancellationToken);
                foreach (var transaction in list)
                    transaction.ExternalId = transaction.Id;
                return list;
            }
                
        }

        public async Task<int> CreateTransactionAsync(CreateTransactionDTO createTransactionDTO, CancellationToken cancellationToken, bool remote, bool updatePiggyBankBalance = true)
        {
            if (!remote)
            {
                var cmd = _mapper.Map<CreateTransactionCommand>(createTransactionDTO);
                cmd.IsExecuteByAdmin = true;
                cmd.InitiatorUserId = 0;
                cmd.UpdatePiggyBankBalance = updatePiggyBankBalance;
                var t = (await ExecuteWithMediator(async mediator => await _mediator.Send(cmd, cancellationToken)));
                return t;
            }
            else 
                return await _integrationService.CreateTransactionOnServerAsync(createTransactionDTO, cancellationToken, updatePiggyBankBalance);
        }

        public async Task UpdateTransactionAsync(int transactionId, UpdateTransactionDTO updateTransactionDTO, CancellationToken cancellationToken, bool remote,  bool updatePiggyBankBalance = true)
        {
            if (!remote)
            {
                var transaction = await ExecuteWithMediator(async mediator => await _mediator.Send(new GetTransactionByIdQuery { Id = transactionId, InitiatorUserId = 0, IsExecuteByAdmin = true }, cancellationToken));
                var result = (await ExecuteWithMediator(async mediator => await _mediator.Send(new UpdateTransactionCommand
                {
                    Id = transaction.Id,
                    InitiatorUserId = 0,
                    Description = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.Description)) ? updateTransactionDTO.Description : transaction.Description,
                    IsExecuteByAdmin = true,
                    Version = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.Version)) ? updateTransactionDTO.Version.Value : transaction.Version,
                    IsDeleted = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.IsDeleted)) ? updateTransactionDTO.IsDeleted : transaction.IsDeleted,
                    Date = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.Date)) ? updateTransactionDTO.Date.Value : transaction.Date,
                    Amount = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.Amount)) ? updateTransactionDTO.Amount.Value : transaction.Amount,
                    PaymentTypeId = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.PaymentTypeId)) ? updateTransactionDTO.PaymentTypeId.Value : transaction.PaymentTypeId,
                    TransactionTypeId = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.TransactionTypeId)) ? updateTransactionDTO.TransactionTypeId.Value : transaction.TransactionTypeId,
                    ExternalId = updateTransactionDTO.EditedFields.Contains(nameof(updateTransactionDTO.ExternalId)) ? updateTransactionDTO.ExternalId : transaction.ExternalId,
                    UpdatePiggyBankBalance = updatePiggyBankBalance
                }, cancellationToken)));
            }
            else
            {
                await _integrationService.UpdateTransactionOnServerAsync(transactionId, updateTransactionDTO, cancellationToken, updatePiggyBankBalance);
            }
        }

        public async Task DeleteTransactionAsync(int transactionId, CancellationToken cancellationToken, bool remote, bool updatePiggyBankBalance = true)
        {
            if (!remote)
            {
                var t = (await ExecuteWithMediator(async mediator => await _mediator.Send(new DeleteTransactionCommand { Id = transactionId, InitiatorUserId = 0, IsExecuteByAdmin = true, UpdatePiggyBankBalance = updatePiggyBankBalance}, cancellationToken)));
            }
            else
                await _integrationService.DeleteTransactionFromServerAsync(transactionId, cancellationToken, updatePiggyBankBalance);
        }

        public async Task<TransactionDTO> RunTwoWayTransactionIntegration(TransactionDTO? localTransaction, TransactionDTO? externalTransaction, CancellationToken cancellationToken)
        {
            return await _integrationService.RunTwoWayTransactionIntegration(localTransaction, externalTransaction, cancellationToken);
        }
    }
}
