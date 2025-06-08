using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
	public class UserPiggyBank
	{
		public int Id { get; set; }
		public int UserId { get; set; }
		public int PiggyBankId { get; set; }
		public bool Public { get; set; }
		public bool HideBalance { get; set; }
        public int? ExternalId { get; set; }
        public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
        public int Version { get; set; }
        public virtual User User { get; set; }
		public virtual PiggyBank PiggyBank { get; set; }
	}
}
