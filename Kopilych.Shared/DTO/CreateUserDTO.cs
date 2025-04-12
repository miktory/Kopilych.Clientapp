using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
	public class CreateUserDTO
	{
		public Guid ExternalUserGuid { get; set; } // keycloak
		public string Username { get; set; }
	}
}
