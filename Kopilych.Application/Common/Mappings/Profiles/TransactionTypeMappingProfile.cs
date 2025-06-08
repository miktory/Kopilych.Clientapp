using AutoMapper;
using Kopilych.Domain;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Mappings.Profiles
{
    public class TransactionTypeMappingProfile: Profile
    {
        public TransactionTypeMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TransactionType, TransactionTypeDTO>();
        }
    }
}
