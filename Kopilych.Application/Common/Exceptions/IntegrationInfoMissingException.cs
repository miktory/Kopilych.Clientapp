using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Exceptions
{
	public class IntegrationInfoMissingException : Exception
	{
        public object Key { get; private set; }
        public IntegrationInfoMissingException() : base($"Integration data is missing.") { }
    }
}
