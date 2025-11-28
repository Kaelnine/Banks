using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;

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
            CreateMap<Core.Models.User, UserDto>().ReverseMap();
            CreateMap<UserDto, Core.Models.User>();
            CreateMap<AccountSummary, AccountSummaryDto>();
        }
    }
}
