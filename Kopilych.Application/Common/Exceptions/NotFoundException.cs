using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Exceptions
{
	public class NotFoundException : Exception
	{
        public object Key { get; private set; }
        public NotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) not found.") { Key = key; }
	}
}
