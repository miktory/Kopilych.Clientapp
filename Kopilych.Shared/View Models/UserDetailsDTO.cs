using System.Text.Json.Serialization;

namespace Kopilych.Shared
{
	public class UserDetailsDTO : ICloneable
	{
		public int Id { get; set; }
        [JsonIgnore]
        public int? ExternalId { get; set; }
        public int Version { get; set; }
        public string? PhotoPath { get; set; }
        public string Username { get; set; }
        //public string Created { get; set; }
        //public string Updated { get; set; }
        [JsonIgnore]
        public bool PhotoIntegrated { get; set; }

        public object Clone()
        {
            return new UserDetailsDTO { Id = Id, ExternalId = ExternalId, PhotoPath = PhotoPath, Version = Version, Username = Username, PhotoIntegrated = PhotoIntegrated };
        }
    }

}
