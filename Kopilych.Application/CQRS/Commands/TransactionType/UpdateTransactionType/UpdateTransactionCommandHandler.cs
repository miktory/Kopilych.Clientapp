using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.TransactionType.UpdateTransactionType
{
    public class UpdateTransactionTypeCommandHandler : IRequestHandler<UpdateTransactionTypeCommand, Unit>
    {
        private readonly ITransactionTypeRepository _repository;
        public UpdateTransactionTypeCommandHandler(ITransactionTypeRepository repository) => _repository = repository;
        public async Task<Unit> Handle(UpdateTransactionTypeCommand request, CancellationToken cancellationToken)
        {
            var transactionType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (transactionType == null)
            {
                throw new NotFoundException(nameof(transactionType), request.Id);
            }

            if (!request.IsExecuteByAdmin)
                throw new AccessDeniedException();

            transactionType.Name = request.Name;

            await _repository.UpdateAsync(transactionType);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
