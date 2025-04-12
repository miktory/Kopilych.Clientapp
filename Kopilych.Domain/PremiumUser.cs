using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Domain
{
    public class PremiumUser
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public virtual User User { get; set; }
    }
}
