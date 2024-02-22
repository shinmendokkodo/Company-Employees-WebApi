﻿using Entities;

namespace Contracts;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompanies(bool trackChanges);
    Task<Company?> GetCompany(Guid companyId, bool trackChanges);
    void CreateCompany(Company company);
    Task<IEnumerable<Company>> GetByIds(IEnumerable<Guid> companyIds, bool trackChanges);
    void DeleteCompany(Company company);
}