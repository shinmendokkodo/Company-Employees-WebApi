using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeDto>> GetEmployees(Guid companyId, bool trackChanges)
    {
        _ = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employees = await repository.Employee.GetEmployees(companyId, trackChanges);
        var employeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employees);

        logger.LogInfo($"All employees for company with id: {companyId} were returned successfully.");
        logger.LogInfo(employeeDtos);

        return employeeDtos;
    }

    public async Task<EmployeeDto> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
    {
        _ = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = await repository.Employee.GetEmployee(companyId, employeeId, trackChanges) ?? throw new EmployeeNotFoundException(employeeId);
        return mapper.Map<EmployeeDto>(employee); 
    }

    public async Task<EmployeeDto> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges) 
    { 
        _ = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = mapper.Map<Employee>(employeeForCreationDto); 
        
        repository.Employee.CreateEmployeeForCompany(companyId, employee); 
        await repository.SaveAsync(); 
        
        return mapper.Map<EmployeeDto>(employee); 
    }

    public async Task<IEnumerable<EmployeeDto>> GetByIds(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges)
    {
        _ = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);

        if (employeeIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var employees = await repository.Employee.GetByIds(companyId, employeeIds, trackChanges);

        if (employeeIds.Count() != employees.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<(IEnumerable<EmployeeDto> companyDtos, string employeeIds)> CreateEmployeeCollection(Guid companyId, IEnumerable<EmployeeForCreationDto> employeeForCreationDtos)
    {
        _ = await repository.Company.GetCompany(companyId, trackChanges: false) ?? throw new CompanyNotFoundException(companyId);

        if (employeeForCreationDtos is null)
        {
            throw new EmployeeCollectionBadRequest();
        }

        var employees = mapper.Map<IEnumerable<Employee>>(employeeForCreationDtos);
        foreach (var employee in employees)
        {
            repository.Employee.CreateEmployeeForCompany(companyId, employee);
        }

        await repository.SaveAsync();

        var employeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employees);
        var employeeIds = string.Join(",", employeeDtos.Select(employee => employee.Id));

        return (employeeDtos, employeeIds);
    }

    public async Task DeleteEmployeeForCompany(Guid companyId, Guid employeeId, bool trackChanges) 
    { 
        _ = await repository.Company.GetCompany(companyId, trackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = await repository.Employee.GetEmployee(companyId, employeeId, trackChanges) ?? throw new EmployeeNotFoundException(employeeId);
        
        repository.Employee.DeleteEmployee(employee); 
        await repository.SaveAsync(); 
    }

    public async Task UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdateDto, bool companyTrackChanges, bool employeeTrackChanges) 
    {
        _ = await repository.Company.GetCompany(companyId, companyTrackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = await repository.Employee.GetEmployee(companyId, employeeId, employeeTrackChanges) ?? throw new EmployeeNotFoundException(employeeId);
        
        mapper.Map(employeeForUpdateDto, employee); 
        await repository.SaveAsync(); 
    }

    public async Task<(EmployeeForUpdateDto employeeForUpdateDto, Employee employee)> GetEmployeeForPatch(Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges)
    {
        _ = await repository.Company.GetCompany(companyId, companyTrackChanges) ?? throw new CompanyNotFoundException(companyId);
        var employee = await repository.Employee.GetEmployee(companyId, employeeId, employeeTrackChanges) ?? throw new EmployeeNotFoundException(companyId);
        
        var employeeForUpdateDto = mapper.Map<EmployeeForUpdateDto>(employee); 
        return (employeeForUpdateDto, employee);
    }

    public async Task SaveChangesForPatch(EmployeeForUpdateDto employeeForUpdateDto, Employee employee) 
    { 
        mapper.Map(employeeForUpdateDto, employee); 
        await repository.SaveAsync(); 
    }
}