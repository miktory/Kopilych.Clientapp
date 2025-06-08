using System.Runtime.Serialization;

namespace Kopilych.Domain
{
	public class PiggyBank
	{
		public int Id { get; set; }
		public int OwnerId { get; set; }
		public decimal Balance { get; set; }
		public decimal? Goal { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public bool Shared { get; set; }
        public DateTime? GoalDate { get; set; } 
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public int Version { get; set; }
        public int? ExternalId { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual User Owner { get; set; }
		public virtual PiggyBankCustomization Customization { get; set; }
        public virtual IEnumerable<UserPiggyBank> Members { get; set; }
        // свойство public вынести в UserAndBankLink
    }
}
