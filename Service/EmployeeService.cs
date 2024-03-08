using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.LinkModels;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service;

internal sealed class EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IEmployeeLinks employeeLinks) : IEmployeeService
{
    public async Task<(LinkResponse linkResponse, Metadata metadata)> GetAllAsync(Guid companyId, EmployeeLinkParameters linkParams, bool trackEmployee)
    {
        if (!linkParams.EmployeeParameters.ValidAgeRange)
        {
            throw new MaxAgeRangeBadRequestException();
        }

        await CheckCompanyExistsAsync(companyId);
        
        var employees = await repository.Employee.GetAllAsync(companyId, linkParams.EmployeeParameters, trackEmployee);
        var employeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employees);
        var linkResponse = employeeLinks.TryGenerateLinks(employeeDtos, linkParams.EmployeeParameters.Fields, companyId, linkParams.Context);

        logger.LogInfo($"All employees for company with id: {companyId} were returned successfully");

        return (linkResponse, employees.Metadata);
    }

    public async Task<EmployeeDto> GetByIdAsync(Guid companyId, Guid employeeId, bool trackEmployee)
    {
        await CheckCompanyExistsAsync(companyId);
        var employee = await ReturnEmployeeIfExistsAsync(companyId, employeeId, trackEmployee);
        var employeeDto = mapper.Map<EmployeeDto>(employee);

        logger.LogInfo("Employee: {employee}", employeeDto);

        return employeeDto;
    }

    public async Task<EmployeeDto> CreateAsync(Guid companyId, EmployeeCreateDto employeeCreateDto)
    {
        await CheckCompanyExistsAsync(companyId);
        var employee = mapper.Map<Employee>(employeeCreateDto);

        repository.Employee.Create(companyId, employee);
        await repository.SaveAsync();

        return mapper.Map<EmployeeDto>(employee);
    }

    public async Task<IEnumerable<EmployeeDto>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> employeeIds, bool trackEmployee)
    {
        await CheckCompanyExistsAsync(companyId);

        if (employeeIds is null)
        {
            throw new IdParametersBadRequestException();
        }

        var employees = await repository.Employee.GetByIdsAsync(companyId, employeeIds, trackEmployee);

        if (employeeIds.Count() != employees.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        return mapper.Map<IEnumerable<EmployeeDto>>(employees);
    }

    public async Task<(IEnumerable<EmployeeDto> employeeDtos, string employeeIds)> CreateCollectionAsync(Guid companyId, IEnumerable<EmployeeCreateDto> employeeCreationDtos)
    {
        await CheckCompanyExistsAsync(companyId);

        if (employeeCreationDtos is null)
        {
            throw new EmployeeCollectionBadRequestException();
        }

        var employees = mapper.Map<IEnumerable<Employee>>(employeeCreationDtos);
        foreach (var employee in employees)
        {
            repository.Employee.Create(companyId, employee);
        }

        await repository.SaveAsync();

        var employeeDtos = mapper.Map<IEnumerable<EmployeeDto>>(employees);
        var employeeIds = string.Join(",", employeeDtos.Select(company => company.Id));

        return (employeeDtos, employeeIds);
    }

    public async Task DeleteAsync(Guid companyId, Guid employeeId, bool trackEmployee)
    {
        await CheckCompanyExistsAsync(companyId);
        var employee = await ReturnEmployeeIfExistsAsync(companyId, employeeId, trackEmployee);

        repository.Employee.Delete(employee);
        await repository.SaveAsync();
    }

    public async Task UpdateAsync(Guid companyId, Guid employeeId, EmployeeUpdateDto employeeUpdateDto, bool trackEmployee)
    {
        await CheckCompanyExistsAsync(companyId);
        var employee = await ReturnEmployeeIfExistsAsync(companyId, employeeId, trackEmployee);

        mapper.Map(employeeUpdateDto, employee);
        await repository.SaveAsync();
    }

    public async Task<(EmployeeUpdateDto employeeUpdateDto, Employee employee)> GetByIdPatchAsync(Guid companyId, Guid employeeId, bool trackEmployee)
    {
        await CheckCompanyExistsAsync(companyId);
        var employee = await ReturnEmployeeIfExistsAsync(companyId, employeeId, trackEmployee);

        var employeeUpdateDto = mapper.Map<EmployeeUpdateDto>(employee);
        return (employeeUpdateDto, employee);
    }

    public async Task SavePatchAsync(EmployeeUpdateDto employeeUpdateDto, Employee employee)
    {
        mapper.Map(employeeUpdateDto, employee);
        await repository.SaveAsync();
    }

    private async Task CheckCompanyExistsAsync(Guid companyId) => 
        _ = await repository.Company.GetByIdAsync(companyId, trackCompany : false) ??
            throw new CompanyNotFoundException(companyId);

    private async Task<Employee> ReturnEmployeeIfExistsAsync(Guid companyId, Guid employeeId, bool trackEmployee) => 
        await repository.Employee.GetByIdAsync(companyId, employeeId, trackEmployee) ?? 
            throw new EmployeeNotFoundException(employeeId);
}