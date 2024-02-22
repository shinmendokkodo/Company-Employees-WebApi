﻿using Contracts;
using Entities;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public IEnumerable<Company> GetAllCompanies(bool trackChanges) =>
        [.. FindAll(trackChanges).OrderBy(c => c.Name)];
    
    public Company? GetCompany(Guid companyId, bool trackChanges) => 
        FindByCondition(c => c.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();
    
    public void CreateCompany(Company company) => Create(company);

    public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges) => 
        [.. FindByCondition(x => ids.Contains(x.Id), trackChanges)];
}