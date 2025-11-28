using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Entities;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;

namespace BanksDB.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Маппинг из InputModel в DTO
            CreateMap<AccountInputModel, AccountDto>();
            CreateMap<TransactionInputModel, TransactionDto>();

            // Маппинг из DTO в OutputModel
            CreateMap<AccountDto, AccountOutputModel>();
            CreateMap<TransactionDto, TransactionOutputModel>();
            CreateMap<AccountSummaryDto, AccountOutputModel>();
            CreateMap<AccountOutputModel, AccountSummaryDto>();
            CreateMap<AccountOutputModel, AccountDto>();

            // Маппинг из ENTITY в DTO
            CreateMap<Account, AccountDto>()
                .ForMember(d => d.OrganizationName, opt => opt.MapFrom(s => s.Organization.Name))
                .ForMember(d => d.BankName, opt => opt.MapFrom(s => s.Bank.Name));

            CreateMap<Organization, OrganizationDto>();
            CreateMap<Bank, BankDto>();
            CreateMap<Transaction, TransactionDto>();

            // Маппинг из DTO в ENTITY
            CreateMap<AccountDto, Account>();
            CreateMap<OrganizationDto, Organization>();
            CreateMap<BankDto, Bank>();
            CreateMap<TransactionDto, Transaction>();

            // Маппинг модели и DTO
            CreateMap<Core.Models.User, UserDto>();
            CreateMap<UserDto, Core.Models.User>();

            // Маппинг из InputModel в ENTITY
            CreateMap<TransactionInputModel, Transaction>();
        }
    }
}
