using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : ICompanyService
{
    public IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges) 
    {
        var companies = repository.Company.GetAllCompanies(trackChanges);
        var companyDtos = mapper.Map<IEnumerable<CompanyDto>>(companies);
        
        logger.LogInfo($"All companies were returned successfully.");
        logger.LogInfo(companyDtos);
        
        return companyDtos;
    }
    
    public CompanyDto GetCompany(Guid id, bool trackChanges)
    {
        var company = repository.Company.GetCompany(id, trackChanges) ?? throw new CompanyNotFoundException(id);
        return mapper.Map<CompanyDto>(company); 
    }

    public CompanyDto CreateCompany(CompanyForCreationDto companyForCreationDto)
    {
        var company = mapper.Map<Company>(companyForCreationDto); 
        
        repository.Company.CreateCompany(company); 
        repository.Save(); 
        
        return mapper.Map<CompanyDto>(company); 
    }

    public IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges) 
    { 
        if (ids is null) 
            throw new IdParametersBadRequestException(); 
        
        var companies = repository.Company.GetByIds(ids, trackChanges); 
        
        if (ids.Count() != companies.Count()) 
            throw new CollectionByIdsBadRequestException(); 
        
        return mapper.Map<IEnumerable<CompanyDto>>(companies);  
    }

    public (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection) 
    {
        if (companyCollection is null) throw new CompanyCollectionBadRequest(); 
        
        var companyEntities = mapper.Map<IEnumerable<Company>>(companyCollection); 
        foreach (var company in companyEntities) 
        { 
            repository.Company.CreateCompany(company); 
        } 
        repository.Save(); 
        
        var companyCollectionToReturn = mapper.Map<IEnumerable<CompanyDto>>(companyEntities); 
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id)); 
        return (companies: companyCollectionToReturn, ids: ids); 
    }
}