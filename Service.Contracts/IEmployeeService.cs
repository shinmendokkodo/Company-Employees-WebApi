using Entities;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace Service.Contracts;

public interface IEmployeeService
{
    Task<(IEnumerable<EmployeeDto> employeeDtos, MetaData metaData)> GetEmployeesAsync(
        Guid companyId,
        EmployeeParameters employeeParameters,
        bool trackChanges
    );

    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);

    Task<EmployeeDto> CreateEmployeeForCompanyAsync(
        Guid companyId,
        EmployeeForCreationDto employeeForCreationDto,
        bool trackChanges
    );

    Task<IEnumerable<EmployeeDto>> GetByIdsAsync(
        Guid companyId,
        IEnumerable<Guid> employeeIds,
        bool trackChanges
    );

    Task<(
        IEnumerable<EmployeeDto> employeeDtos,
        string employeeIds
    )> CreateEmployeesForCompanyCollectionAsync(
        Guid companyId,
        IEnumerable<EmployeeForCreationDto> employeeForCreationDtos
    );

    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges);

    Task UpdateEmployeeForCompanyAsync(
        Guid companyId,
        Guid employeeId,
        EmployeeForUpdateDto employeeForUpdateDto,
        bool companyTrackChanges,
        bool employeeTrackChanges
    );

    Task<(EmployeeForUpdateDto employeeForUpdateDto, Employee employee)> GetEmployeeForPatchAsync(
        Guid companyId,
        Guid employeeId,
        bool companyTrackChanges,
        bool employeeTrackChanges
    );
    
    Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeForUpdateDto, Employee employee);
}
