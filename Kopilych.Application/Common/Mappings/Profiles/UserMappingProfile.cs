using AutoMapper;
using Kopilych.Domain;
using Kopilych.Shared;
using Kopilych.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kopilych.Application.Common.Mappings.Profiles
{
	public class UserMappingProfile : Profile
	{
		public UserMappingProfile()
		{
			Mapping(this);
		}

		public void Mapping(Profile profile)
		{
			profile.CreateMap<User, UserDetailsDTO>();
            profile.CreateMap<UserDetailsDTO, UpdateUserDTO>();
        }
	}
}
