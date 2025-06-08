using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.View_Models
{
    public class PiggyBankDTO
    {
        private DateTime? _goalDate;
        private DateTime _created;
        private DateTime _updated;
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public decimal Balance { get; set; }
        public decimal? Goal { get; set; }
        public string Name 
        { 
            get; 
            set; }
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
        public DateTime Created
        {
            get
            {

                return _created.ToUniversalTime();
            }
            set
            {
                _created = value.ToUniversalTime();
               
            }
        }
        public DateTime Updated
        {
            get
            {
                    
                return _updated.ToUniversalTime();
            }
            set
            {
                _updated = value.ToUniversalTime();
            }
        }
        public int Version { get; set; }
        public int Percentage { get; set; }
        public bool? IsDeleted {  get; set; }

        public object Clone()
        {
            return new PiggyBankDTO { Id = Id, ExternalId = ExternalId, OwnerId = OwnerId, Balance = Balance, Version = Version, Created = Created, Description = Description, Goal = Goal, GoalDate = GoalDate, IsDeleted = IsDeleted, Name = Name, Percentage = Percentage, Shared = Shared, Updated = Updated };
        }
    }
}
