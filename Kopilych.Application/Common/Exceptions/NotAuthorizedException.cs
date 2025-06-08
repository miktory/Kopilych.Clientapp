using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Exceptions
{
	public class AccessDeniedException : Exception
	{
		public AccessDeniedException() : base($"The operation is not available for the current access rights state.") {  }
	}
}
