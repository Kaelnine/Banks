using AutoMapper;
using BanksDB.BLL.Interfaces;
using BanksDB.Core.Entities;
using BanksDB.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanksDB.BLL.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMapper _mapper;
        public OrganizationService(IOrganizationRepository  organizationRepository, IMapper mapper)
        {
            _organizationRepository = organizationRepository;
            _mapper = mapper;
        }

        public async Task AddOrganizationAsync(Organization organization)
        {
            await _organizationRepository.AddAsync(organization);
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            await _organizationRepository.DeleteAsync(id);
        }

        public async Task<List<Organization>> GetAllOrganizationsAsync()
        {
            var organizations = await _organizationRepository.GetAllAsync();
            return _mapper.Map<List<Organization>>(organizations);
        }

        public async Task<Organization> GetOrganizationByIdAsync(int id)
        {
            var organization = await _organizationRepository.GetByIdAsync(id);
            return _mapper.Map<Organization>(organization);
        }

        public async Task UpdateOrganizationAsync(Organization organization)
        {
            await _organizationRepository.UpdateAsync(organization);
        }
    }
}
