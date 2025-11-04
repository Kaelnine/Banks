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
            CreateMap<TransactionDto, TransactionOutputModel>();
            CreateMap<AccountSummaryDto, AccountOutputModel>();
            CreateMap<AccountOutputModel, AccountSummaryDto>();
            CreateMap<AccountOutputModel, AccountDto>();
        }
    }
}
