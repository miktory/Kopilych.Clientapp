using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
	public class User
	{
		public int Id { get; set; }
		public Guid ExternalUserGuid { get; set; } // keycloak
		public string Username { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public string PhotoPath { get; set; }
        public int Version { get; set; }
        public virtual ICollection<PiggyBank> PiggyBanks { get; set; } = new List<PiggyBank>();
		public virtual ICollection<UserFriendship> FriendshipsAsInitiator { get; set; } = new List<UserFriendship>();
		public virtual ICollection<UserFriendship> FriendshipsAsApprover { get; set; } = new List<UserFriendship>();
		public virtual ICollection<UserPiggyBank> UserPiggyBankRecords { get; set; } = new List<UserPiggyBank>();
		public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
	}
}
