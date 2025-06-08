using AutoMapper;
using Kopilych.Domain;
using Kopilych.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Mappings.Profiles
{
    public class UserSessionMappingProfile : Profile
    {
        public UserSessionMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<UserSession, UserSessionDTO>();
            profile.CreateMap<UserSession, SessionDTO>();
            profile.CreateMap<UserSessionDTO, SessionDTO>();
            profile.CreateMap<SessionDTO, RefreshSessionDTO>();
            profile.CreateMap<UserSessionDTO, RefreshSessionDTO>();
            profile.CreateMap<UserSessionDTO, LogoutDTO>();
            profile.CreateMap<SessionDTO, LogoutDTO>();
        }
    }
}
