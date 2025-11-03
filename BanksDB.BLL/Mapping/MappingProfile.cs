using AutoMapper;
using BanksDB.Core.Dtos;
using BanksDB.Core.Models.InputModels;
using BanksDB.Core.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //CreateMap<AccountDto, AccountOutputModel>()
            //    .ForMember(dest => dest.OrganizationName,
            //        opt => opt.MapFrom(src => src.Organization != null ? src.Organization.Name : src.OrganizationName))
            //    .ForMember(dest => dest.BankName,
            //        opt => opt.MapFrom(src => src.Bank != null ? src.Bank.Name : src.BankName));
            CreateMap<TransactionDto, TransactionOutputModel>();
            CreateMap<AccountSummaryDto, AccountOutputModel>();
            CreateMap<AccountOutputModel, AccountSummaryDto>();
            CreateMap<AccountOutputModel, AccountDto>();
        }
    }
}
