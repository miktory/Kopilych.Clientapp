namespace Kopilych.WebApi
{
    public class KeycloakSettings
    {
        public string Address { get; set; }
        public string Realm { get; set; }
        public string Audience { get; set; }
        public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public bool RequireHttpsMetadata { get; set; }
        public string RedirectUri { get; set; }
        public string PublicKey { get; set; }
    }
}
