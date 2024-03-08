using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface IEmployeeService
{
    Task<(LinkResponse linkResponse, Metadata metadata)> GetAllAsync(Guid companyId, EmployeeLinkParameters linkParams, bool trackEmployee = false);

    Task<EmployeeDto> GetByIdAsync(Guid companyId, Guid employeeId, bool trackEmployee = false);
    
    Task<IEnumerable<EmployeeDto>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> employeeIds, bool trackEmployee = false);
    
    Task<(EmployeeUpdateDto employeeUpdateDto, Employee employee)> GetByIdPatchAsync(Guid companyId, Guid employeeId, bool trackEmployee = true);

    Task<EmployeeDto> CreateAsync(Guid companyId, EmployeeCreateDto employeeCreateDto);

    Task<(IEnumerable<EmployeeDto> employeeDtos, string employeeIds)> CreateCollectionAsync(Guid companyId, IEnumerable<EmployeeCreateDto> employeeCreateDtos);

    Task DeleteAsync(Guid companyId, Guid employeeId, bool trackEmployee = false);

    Task UpdateAsync(Guid companyId, Guid employeeId, EmployeeUpdateDto employeeUpdateDto, bool trackEmployee = true);

    Task SavePatchAsync(EmployeeUpdateDto employeeUpdateDto, Employee employee);
}