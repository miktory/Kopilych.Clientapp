using Kopilych.Application.Common.Exceptions;
using Kopilych.Application.Interfaces.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBankType.UpdatePiggyBankType
{
    public class UpdatePiggyBankTypeCommandHandler : IRequestHandler<UpdatePiggyBankTypeCommand, Unit>
    {
        private readonly IPiggyBankTypeRepository _repository;
        public UpdatePiggyBankTypeCommandHandler(IPiggyBankTypeRepository repository) => _repository = repository;
        public async Task<Unit> Handle(UpdatePiggyBankTypeCommand request, CancellationToken cancellationToken)
        {
            var piggyBankType = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (piggyBankType == null)
            {
                throw new NotFoundException(nameof(piggyBankType), request.Id);
            }

            if (!request.IsExecuteByAdmin)
                throw new AccessDeniedException();

            piggyBankType.Name = request.Name;
            piggyBankType.FirstStatePhotoPath = request.FirstStatePhotoPath;
            piggyBankType.SecondStatePhotoPath = request.SecondStatePhotoPath;
            piggyBankType.ThirdStatePhotoPath = request.ThirdStatePhotoPath;
            piggyBankType.FourthStatePhotoPath = request.FourthStatePhotoPath;

            await _repository.UpdateAsync(piggyBankType);
            await _repository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }

    }
}
