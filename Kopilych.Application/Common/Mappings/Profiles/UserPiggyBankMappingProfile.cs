using AutoMapper;
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
    public class UserPiggyBankMappingProfile : Profile
    {
        public UserPiggyBankMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserPiggyBank, UserPiggyBankDTO>();
            profile.CreateMap<UserPiggyBankDTO, CreateUserPiggyBankDTO>();
            profile.CreateMap<UserPiggyBankDTO, UpdateUserPiggyBankDTO>();
        }
    }
}
