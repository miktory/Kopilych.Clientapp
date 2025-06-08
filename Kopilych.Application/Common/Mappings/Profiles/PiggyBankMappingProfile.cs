using AutoMapper;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Mappings.Profiles
{
    public class PiggyBankMappingProfile : Profile
    {
        public PiggyBankMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<PiggyBank, PiggyBankDTO>()
                .ForMember(vm => vm.Percentage, pb => pb.MapFrom(pb => (pb.Balance > 0 && pb.Goal > 0) ? (int)((pb.Balance / pb.Goal) * 100) : 0));

            profile.CreateMap<PiggyBankDTO, CreatePiggyBankDTO>();
            profile.CreateMap<PiggyBankDTO, UpdatePiggyBankDTO>();
        }
    }
}
