using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Exceptions
{
	public class NotAuthorizedException : Exception
	{
		public NotAuthorizedException() : base($"Session is not valid.") {  }
	}
}
