using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PaymentType.UpdatePaymentType
{
    public class UpdatePaymentTypeCommandHandler : IRequestHandler<UpdatePaymentTypeCommand, Unit>
    {
        private readonly IPaymentTypeRepository _repository;
        public UpdatePaymentTypeCommandHandler(IPaymentTypeRepository repository) => _repository = repository;
        public async Task<Unit> Handle(UpdatePaymentTypeCommand request, CancellationToken cancellationToken)
        {
            var paymentType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (paymentType == null)
            {
                throw new NotFoundException(nameof(paymentType), request.Id);
            }

            if (!request.IsExecuteByAdmin)
                throw new AccessDeniedException();

            paymentType.Name = request.Name;

            await _repository.UpdateAsync(paymentType);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
