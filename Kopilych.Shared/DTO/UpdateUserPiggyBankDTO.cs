using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdateUserPiggyBankDTO
    {
        private int? _version;
        private bool? _public;
        private int? _externalId;
        private bool? _hideBalance;

        [JsonIgnore]
        public List<string> EditedFields { get; set; } = new List<string>();


        public bool? HideBalance
        {
            get { return _hideBalance; }
            set { _hideBalance = value; EditedFields.Add(nameof(this.HideBalance)); }
        }

        [JsonIgnore]
        public int? ExternalId
        {
            get { return _externalId; }
            set { _externalId = value; EditedFields.Add(nameof(this.ExternalId)); }
        }
        public bool? Public
        {
            get { return _public; }
            set { _public = value; EditedFields.Add(nameof(this.Public)); }
        }
        public int? Version
        {
            get { return _version; }
            set { _version = value ?? 0; EditedFields.Add(nameof(this.Version)); }
        }
    }
}
