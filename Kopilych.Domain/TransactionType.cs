﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
	public class TransactionType
	{
		public int Id { get; set; }
		public string Name { get; set; }

        public bool IsPositive { get; set; }
    }
}
