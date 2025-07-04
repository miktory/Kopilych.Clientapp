﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kopilych.Shared.View_Models
{
    public class UserPiggyBankDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PiggyBankId { get; set; }
        public int Version { get; set; }
        [JsonIgnore]
        public int? ExternalId { get; set; }
        // public int OwnerId { get; set; }
        //  public decimal Balance { get; set; }
        //  public decimal? Goal { get; set; }
        //  public string Name { get; set; }
        //  public string? Description { get; set; }
        //  public bool Shared { get; set; }
        public bool HideBalance { get; set; }
      //  public DateTime? GoalDate { get; set; }
     //   public DateTime Created { get; set; }
     //   public DateTime Updated { get; set; }
     //   public int Version { get; set; }
    //    public int Percentage { get; set; }
        public bool Public { get; set; }

        public object Clone()
        {
            return new UserPiggyBankDTO { Id = Id, ExternalId = ExternalId, Version = Version, HideBalance = HideBalance, PiggyBankId = PiggyBankId, Public = Public, UserId = UserId };
        }
    }
}
