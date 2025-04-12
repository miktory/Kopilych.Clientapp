using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Mobile.Models
{
    public class User
    {
        public int Id { get; set; }
        public int ExternalUserId { get; set; } // Integration UserId
        public int Version { get; set; }
        public string Username { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string PhotoPath { get; set; }
    }
}
