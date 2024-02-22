using AutoMapper;
using Contracts;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : ICompanyService
{
    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges) 
    {
        var companies = repository.Company.GetAllCompanies(trackChanges);
        var companiesDto = mapper.Map<IEnumerable<CompanyDto>>(companies);
        return companiesDto;
    }
}