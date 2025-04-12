using AutoMapper;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.View_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Mappings.Profiles
{
    public class UserFriendshipMappingProfile : Profile
    {
        public UserFriendshipMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserFriendship, UserFriendshipDetailsVm>();
        }
    }
}
