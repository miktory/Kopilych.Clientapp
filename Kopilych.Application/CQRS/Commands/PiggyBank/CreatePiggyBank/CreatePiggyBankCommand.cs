using Kopilych.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBank.CreatePiggyBank
{
    public class CreatePiggyBankCommand : IRequest<int>
    {
        public int OwnerId { get; set; }
        public int Version { get; set; }
        public decimal Balance { get; set; }
        public decimal? Goal { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Shared { get; set; }
        public DateTime? GoalDate { get; set; }
        public bool IsExecuteByAdmin {get; set;}
        public int InitiatorUserId { get; set; }
    }
}
