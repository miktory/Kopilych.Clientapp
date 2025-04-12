using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdatePiggyBankDTO
    {
        private string _name;
        private string? _description;
        private bool _shared;
        private DateTime? _goalDate;
        private decimal? _balance;
        private int? _version;
        private decimal? _goal;

        [JsonIgnore]
        public List<string> EditedFields { get; set; } = new List<string>();

        public int? Version {
            get { return _version; }
            set { _version = value ?? 0; EditedFields.Add(nameof(this.Version)); }
        }
        public decimal? Balance
        {
            get { return _balance; }
            set { _balance = value?? 0; EditedFields.Add(nameof(this.Balance)); }
        }

        public decimal? Goal {

            get { return _goal; }
            set { _goal = value; EditedFields.Add(nameof(this.Goal)); }
        }

        [MinLength(1)]
        public string? Name
        {
            get { return _name; }
            set { _name = value ?? string.Empty; EditedFields.Add(nameof(this.Name)); } 
        }
        public string? Description 
        {
            get { return _description; }
            set { _description = value; EditedFields.Add(nameof(this.Description)); }
        }
        public bool? Shared
        {
            get { return _shared; }
            set { _shared = value ?? false; EditedFields.Add(nameof(this.Shared)); }
        }
        public DateTime? GoalDate 
        {
            get { return _goalDate; }
            set { _goalDate = value; EditedFields.Add(nameof(this.GoalDate)); }
        }
    }
}
