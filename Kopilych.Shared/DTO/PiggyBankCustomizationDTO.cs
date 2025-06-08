using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class PiggyBankCustomizationDTO
    {
        public int Id {  get; set; }
        public int PiggyBankId { get; set; }
        public string? PhotoPath { get; set; }
        public int PiggyBankTypeId { get; set; }
        [JsonIgnore]
        public int? ExternalId { get; set; }
        public int Version { get; set; }
        [JsonIgnore]
        public bool PhotoIntegrated { get; set; }

        public object Clone()
        {
            return new PiggyBankCustomizationDTO { Id = Id, ExternalId = ExternalId, PhotoPath = PhotoPath, Version = Version, PiggyBankId = PiggyBankId, PiggyBankTypeId = PiggyBankTypeId, PhotoIntegrated = PhotoIntegrated };
        }


    }
}
