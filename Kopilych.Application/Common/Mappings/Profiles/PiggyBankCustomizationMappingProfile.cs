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
    public class PiggyBankCustomizationMappingProfile: Profile
    {
        public PiggyBankCustomizationMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PiggyBankCustomization, PiggyBankCustomizationDTO>();
            profile.CreateMap<PiggyBankCustomizationDTO, CreatePiggyBankCustomizationDTO>();
            profile.CreateMap<PiggyBankCustomizationDTO, UpdatePiggyBankCustomizationDTO>();
        }
    }
}
