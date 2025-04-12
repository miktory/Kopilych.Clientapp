using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Kopilych.Application.Interfaces;
using Kopilych.Application.Common.Mappings.Profiles;
using System.Security.Cryptography;

namespace Kopilych.Application.Common.Mappings
{
    public class MappingConfiguration : Profile
    {
        public MappingConfiguration(IMapperConfigurationExpression cfg) =>
            ApplyMappings(cfg);

        public void ApplyMappings(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<UserFriendshipMappingProfile>();
            cfg.AddProfile<PiggyBankMappingProfile>();
            cfg.AddProfile<UserPiggyBankMappingProfile>();
            cfg.AddProfile<UpdatePiggyBankDtoMappingProfile>();
            cfg.AddProfile<PaymentTypeMappingProfile>();
            cfg.AddProfile<PiggyBankTypeMappingProfile>();
            cfg.AddProfile<TransactionTypeMappingProfile>();
            cfg.AddProfile<TransactionMappingProfile>();
            cfg.AddProfile<PiggyBankCustomizationMappingProfile>();
            // Здесь можно добавить другие профили
        }
    }
}
