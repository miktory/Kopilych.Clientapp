using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class UpdateUserDTO
    {
        public string? Username { get; set;}
        public int? Version {  get; set;}
        [JsonIgnore]
        public int? ExternalId { get; set; }
        [JsonIgnore]
        public string? PhotoPath { get; set; }

        [JsonIgnore]
        public bool? PhotoIntegrated { get; set; }
    }
}
