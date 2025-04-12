using AutoMapper;
using Kopilych.Application.CQRS.Commands.PiggyBank.UpdatePiggyBank;
using Kopilych.Domain;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Mappings.Profiles
{
    public class UpdatePiggyBankDtoMappingProfile: Profile
    {
        public UpdatePiggyBankDtoMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UpdatePiggyBankDTO, UpdatePiggyBankCommand>();
        }
    }
}
