using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank
{
    public class UpdatePiggyBankCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int Version { get; set; }
        public decimal Balance { get; set; }
        public decimal? Goal { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ExternalId { get; set; }
        public bool Shared { get; set; }
        public DateTime? GoalDate { get; set; }

        public int InitiatorUserId { get; set; }
        public bool IsExecuteByAdmin { get; set; }  
    }
}
