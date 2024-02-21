﻿using Contracts;
using Entities;

namespace Repository;

public class CompanyRepository(RepositoryContext repositoryContext) : RepositoryBase<Company>(repositoryContext), ICompanyRepository
{
    public IEnumerable<Company> GetAllCompanies(bool trackChanges) => 
        FindAll(trackChanges)
            .OrderBy(c => c.Name)
            .ToList();
}