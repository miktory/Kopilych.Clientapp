using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreatePiggyBankDTO

    {
        public int OwnerId { get; set; }
        public int Version { get; set; }
        public decimal Balance { get; set; }
        public decimal? Goal { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Shared { get; set; }
        public DateTime? GoalDate { get; set; }
    }
}
