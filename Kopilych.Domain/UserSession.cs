using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
    public class UserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public virtual User User { get; set; }
    }
}
