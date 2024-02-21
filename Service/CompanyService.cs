using AutoMapper;
using Contracts;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : ICompanyService
{
    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges) 
    {
        try
        {
            var companies = repository.Company.GetAllCompanies(trackChanges);
            var companiesDto = mapper.Map<IEnumerable<CompanyDto>>(companies);
            return companiesDto;
        } 
        catch (Exception ex) 
        { 
            logger.LogError($"Something went wrong in the {nameof(GetAllCompanies)} service method {ex}");
            throw; 
        }
    }
}