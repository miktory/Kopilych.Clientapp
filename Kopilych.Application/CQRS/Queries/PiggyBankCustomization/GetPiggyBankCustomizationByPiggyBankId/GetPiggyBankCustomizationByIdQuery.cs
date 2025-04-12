using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PiggyBankCustomization.GetPiggyBankCustomizationByPiggyBankId
{
    public class GetPiggyBankCustomizationByPiggyBankIdQuery : IRequest<PiggyBankCustomizationDTO>
    {
        public int PiggyBankId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public int InitiatorUserId { get; set; }
    }
}
