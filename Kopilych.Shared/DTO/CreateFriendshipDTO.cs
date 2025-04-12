using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Shared.DTO
{
    public class CreateFriendshipDTO
    {
        [Required]
        public int InitiatorUserId { get; set; }
        [Required]
        public int ApproverUserId { get; set; }
    }
}
