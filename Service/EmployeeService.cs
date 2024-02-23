using AutoMapper;
using Contracts;
using Entities;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper) : IEmployeeService
{
    public async Task<(IEnumerable<EmployeeDto> employeeDtos, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    {
        if (!employeeParameters.ValidAgeRange)
        {
            throw new MaxAgeRangeBadRequestException();
        }

        await CheckIfCompanyExists(companyId, trackChanges);
        var employees = await repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
        var employeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employees);

        logger.LogInfo($"All employees for company with id: {companyId} were returned successfully.");
        logger.LogInfo(employeeDtos);

        return (employeeDtos, employees.MetaData);
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);
        return mapper.Map<EmployeeDto>(employee); 
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreationDto, bool trackChanges) 
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        var employee = mapper.Map<Employee>(employeeForCreationDto); 
        
        repository.Employee.CreateEmployeeForCompany(companyId, employee); 
        await repository.SaveAsync(); 
        
        return mapper.Map<EmployeeDto>(employee); 
    }

    public async Task<IEnumerable<EmployeeDto>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> employeeIds, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        if (employeeIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var employees = await repository.Employee.GetByIdsAsync(companyId, employeeIds, trackChanges);

        if (employeeIds.Count() != employees.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<(IEnumerable<EmployeeDto> employeeDtos, string employeeIds)> CreateEmployeesForCompanyCollectionAsync(Guid companyId, IEnumerable<EmployeeForCreationDto> employeeForCreationDtos)
    {
        await CheckIfCompanyExists(companyId, trackChanges: false);

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
        var employeeIds = string.Join(",", employeeDtos.Select(company => company.Id));

        return (employeeDtos, employeeIds);
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges) 
    {
        await CheckIfCompanyExists(companyId, trackChanges);
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);
        
        repository.Employee.DeleteEmployee(employee); 
        await repository.SaveAsync(); 
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid employeeId, EmployeeForUpdateDto employeeForUpdateDto, bool companyTrackChanges, bool employeeTrackChanges) 
    {
        await CheckIfCompanyExists(companyId, trackChanges: false);
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, employeeTrackChanges);
        
        mapper.Map(employeeForUpdateDto, employee); 
        await repository.SaveAsync(); 
    }

    public async Task<(EmployeeForUpdateDto employeeForUpdateDto, Employee employee)> GetEmployeeForPatchAsync(Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges: false);
        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, employeeTrackChanges);
        
        var employeeForUpdateDto = mapper.Map<EmployeeForUpdateDto>(employee); 
        return (employeeForUpdateDto, employee);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeForUpdateDto, Employee employee) 
    { 
        mapper.Map(employeeForUpdateDto, employee); 
        await repository.SaveAsync(); 
    }

    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges) => 
        _ = await repository.Company.GetCompanyAsync(companyId, trackChanges) 
        ?? throw new CompanyNotFoundException(companyId);

    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid employeeId, bool trackChanges) => 
        await repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges) 
        ?? throw new EmployeeNotFoundException(employeeId);
}
