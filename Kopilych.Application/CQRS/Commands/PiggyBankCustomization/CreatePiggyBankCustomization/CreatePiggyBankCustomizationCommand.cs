using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBankCustomization.CreatePiggyBankCustomization
{
    public class CreatePiggyBankCustomizationCommand : IRequest<int>
    {
        public int PiggyBankId { get; set; }
        public string? PhotoPath { get; set; }
        public int PiggyBankTypeId { get; set; }
        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }
        public int? ExternalId { get; set; }
        public bool PhotoIntegrated { get; set; }
        public int Version { get; set; }
    }
}
