using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Mapping
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Account, AccountDto>().ReverseMap();
            CreateMap<Bank, BankDto>().ReverseMap();
            CreateMap<Organization, OrganizationDto>().ReverseMap();
            CreateMap<Transaction, TransactionDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<AccountSummary, AccountSummaryDto>();
        }
    }
}
