using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{

    public class UpdatePiggyBankCustomizationDTO
    {
        private int? _version;
        private int? _piggyBankTypeId;
        private string? _photoPath;
        private int? _externalId;
        private bool? _photoIntegrated;

        [JsonIgnore]
        public List<string> EditedFields { get; set; } = new List<string>();

        [JsonIgnore]
        public int? ExternalId
        {
            get { return _externalId; }
            set { _externalId = value ?? 0; EditedFields.Add(nameof(this.ExternalId)); }
        }
        public int? Version
        {
            get { return _version; }
            set { _version = value ?? 0; EditedFields.Add(nameof(this.Version)); }
        }

        public int? PiggyBankTypeId
        {
            get { return _piggyBankTypeId; }
            set { _piggyBankTypeId = value ?? 0; EditedFields.Add(nameof(this.PiggyBankTypeId)); }
        }

        public string? PhotoPath
        {
            get { return _photoPath; }
            set { _photoPath = value; EditedFields.Add(nameof(this.PhotoPath)); }
        }

        [JsonIgnore]
        public bool? PhotoIntegrated
        {
            get { return _photoIntegrated; }
            set { _photoIntegrated = value; EditedFields.Add(nameof(this.PhotoIntegrated)); }
        }
    }
}
