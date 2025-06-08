using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreateUserPiggyBankDTO
    {
        public int UserId { get; set; }
        public int PiggyBankId { get; set; }
        [JsonIgnore]
        public int? ExternalId { get; set; }
        public int Version { get; set; }
        public bool HideBalance { get; set; }
        public bool Public { get; set; }
        //   public bool HideBalance { get; set; }
        //    public bool Public { get; set; }
    }
}
