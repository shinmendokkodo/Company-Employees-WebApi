using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : IEmployeeService
{
    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    {
        _ = repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employees = repository.Employee.GetEmployees(companyId, trackChanges);
        var employeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employees);

        logger.LogInfo($"All employees for company with id: {companyId} were returned successfully.");
        logger.LogInfo(employeeDtos);

        return employeeDtos;
    }

    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    {
        _ = repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = repository.Employee.GetEmployee(companyId, id, trackChanges) ?? throw new EmployeeNotFoundException(id);
        return mapper.Map<EmployeeDto>(employee); 
    }

    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges) 
    { 
        _ = repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = mapper.Map<Employee>(employeeForCreationDto); 
        
        repository.Employee.CreateEmployeeForCompany(companyId, employee); 
        repository.Save(); 
        
        return mapper.Map<EmployeeDto>(employee); 
    }
}