using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Queries.PiggyBankCustomization.GetPiggyBankCustomizationById
{
    public class GetPiggyBankCustomizationByIdQuery : IRequest<PiggyBankCustomizationDTO>
    {
        public int Id { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public int InitiatorUserId { get; set; }
    }
}
