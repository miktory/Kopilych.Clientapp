﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
	public class UserFriendship
	{
		public int Id { get; set; }
		public int InitiatorUserId { get; set; }
		public int ApproverUserId { get; set; }
		public bool RequestApproved { get; set; }
		public DateTime Created { get; set; }
		public DateTime Updated { get; set; }
		public virtual User InitiatorUser { get; set; }
		public virtual User ApproverUser { get; set; }
	}
}
