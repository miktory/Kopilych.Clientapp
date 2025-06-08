using AutoMapper;
using Kopilych.Application.CQRS.Commands.Transaction.CreateTransaction;
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
    public class TransactionMappingProfile: Profile
    {
        public TransactionMappingProfile()
        {
            Mapping(this);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Transaction, TransactionDTO>();
            profile.CreateMap<TransactionDTO, CreateTransactionDTO>();
            profile.CreateMap<TransactionDTO, UpdateTransactionDTO>();
            profile.CreateMap<CreateTransactionDTO, CreateTransactionCommand>();
        }
    }
}
