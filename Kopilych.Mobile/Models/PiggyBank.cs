using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Models
{
    public class PiggyBank
    {
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public decimal Balance { get; set; }
        public decimal? Goal { get; set; }
        public string Name { get; set; }
        public int? ExternalId { get; set; }
        public string? Description { get; set; }
        public bool Shared { get; set; }
        public DateTime? GoalDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Version { get; set; }
        public int Percentage { get; set; }
    }
}
