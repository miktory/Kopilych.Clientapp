using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PaymentType.GetAllPaymentTypes
{
    public class GetAllPaymentTypesQuery : IRequest<List<PaymentTypeDTO>>
    {
        public int InitiatorUserId {  get; set; }
        public bool IsExecuteByAdmin {  get; set; }
    }
}
