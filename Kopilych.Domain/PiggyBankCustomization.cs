using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
	public class PiggyBankCustomization
	{
		public int Id { get; set; }
		public int PiggyBankId { get; set; }
		public int PiggyBankTypeId { get; set; }
		public string PhotoPath { get; set; }
        public int? ExternalId { get; set; }
        public int Version { get; set; }
        public bool PhotoIntegrated { get; set; }
        public virtual PiggyBankType PiggyBankType { get; set; }
		public virtual PiggyBank PiggyBank { get; set; }
	}
}
