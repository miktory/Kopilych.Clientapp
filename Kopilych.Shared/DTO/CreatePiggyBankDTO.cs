using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreatePiggyBankDTO
    {
        private DateTime? _goalDate;
        public int OwnerId { get; set; }
        public int Version { get; set; }
        public decimal Balance { get; set; }
        public decimal? Goal { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Shared { get; set; }
        [JsonIgnore]
        public int? ExternalId { get; set; }
        public DateTime? GoalDate
        {
            get
            {
                return _goalDate.HasValue ? _goalDate.Value.ToUniversalTime() : _goalDate;
            }
            set
            {
                _goalDate = value.HasValue ? value.Value.ToUniversalTime() : value;
            }
        }
    }
}
