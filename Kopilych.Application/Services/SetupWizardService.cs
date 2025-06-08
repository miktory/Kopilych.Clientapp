using AutoMapper;
using Kopilych.Application.CQRS.Commands.PaymentType.CreatePaymentType;
using Kopilych.Application.CQRS.Commands.PiggyBankType.CreatePiggyBankType;
using Kopilych.Application.CQRS.Commands.TransactionType.CreateTransactionType;
using Kopilych.Application.CQRS.Queries.PaymentType.GetAllPaymentTypes;
using Kopilych.Application.CQRS.Queries.PiggyBankType.GetAllPiggyBankTypes;
using Kopilych.Application.CQRS.Queries.TransactionType.GetAllTransactionTypes;
using Kopilych.Application.Interfaces;
using Kopilych.Domain;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Services
{
    public class SetupWizardService : ISetupWizardService
    {
        private IMediator _mediator;
        private IMapper _mapper;

        public SetupWizardService(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        public async Task ConfigureAsync()
        {
            // конфигурируем без cancellation token, т.к. процесс обязательно должен идти до конца
            var piggyBankTypes = await _mediator.Send(new GetAllPiggyBankTypesQuery { IsExecuteByAdmin = true });
            var requiredPiggyBankTypes = new List<PiggyBankType> { new PiggyBankType { Id = 1, Name = "Баночка", FirstStatePhotoPath = "/PiggyBankType/Jar/jar_state1.png", SecondStatePhotoPath = "/PiggyBankType/Jar/jar_state2.png", ThirdStatePhotoPath = "/PiggyBankType/Jar/jar_state3.png", FourthStatePhotoPath = "/PiggyBankType/Jar/jar_state4.png" },
                new Domain.PiggyBankType { Id = 2, Name = "Свинка-копилка", FirstStatePhotoPath = "/PiggyBankType/PiggyBank/piggybank_state1.png", SecondStatePhotoPath = "/PiggyBankType/PiggyBank/piggybank_state2.png", ThirdStatePhotoPath = "/PiggyBankType/PiggyBank/piggybank_state3.png", FourthStatePhotoPath = "/PiggyBankType/PiggyBank/piggybank_state4.png" } };
            foreach (var type in requiredPiggyBankTypes)
            {
                if(!piggyBankTypes.Contains(_mapper.Map<PiggyBankTypeDTO>(type)))
                    await _mediator.Send(new CreatePiggyBankTypeCommand { IsExecuteByAdmin = true, Name = type.Name, Id = type.Id, FirstStatePhotoPath = type.FirstStatePhotoPath, SecondStatePhotoPath = type.SecondStatePhotoPath, ThirdStatePhotoPath = type.ThirdStatePhotoPath, FourthStatePhotoPath = type.FourthStatePhotoPath, InitiatorUserId = 0});
            }

            var transactionTypes = await _mediator.Send(new GetAllTransactionTypesQuery { IsExecuteByAdmin = true, InitiatorUserId = 0});
            var paymentTypes = await _mediator.Send(new GetAllPaymentTypesQuery { IsExecuteByAdmin = true, InitiatorUserId = 0 });
            var requiredPaymentTypes = new List<PaymentType> { new PaymentType { Id = 1, Name = "Наличные" }, new PaymentType { Id = 2, Name = "Перевод" } };
            var requiredTransactionTypes = new List<TransactionType> { new TransactionType { Id = 1, Name = "Пополнение", IsPositive = true }, new TransactionType { Id = 2, Name = "Списание", IsPositive = false } };
            foreach (var type in requiredTransactionTypes)
            {
                if (!transactionTypes.Contains(_mapper.Map<TransactionTypeDTO>(type)))
                    await _mediator.Send(new CreateTransactionTypeCommand { IsExecuteByAdmin = true, Name = type.Name, Id = type.Id, IsPositive = type.IsPositive });
            }

            foreach (var type in requiredPaymentTypes)
            {
                if (!paymentTypes.Contains(_mapper.Map<PaymentTypeDTO>(type)))
                    await _mediator.Send(new CreatePaymentTypeCommand { IsExecuteByAdmin = true, Name = type.Name, Id = type.Id });
            }

        }
    }
}
