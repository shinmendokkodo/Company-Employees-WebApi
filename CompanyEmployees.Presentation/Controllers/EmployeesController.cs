using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmployeesForCompany(
        Guid companyId,
        [FromQuery] EmployeeParameters employeeParameters
    )
    {
        var (employeeDtos, metaData) = await service.EmployeeService.GetEmployeesAsync(
            companyId,
            employeeParameters,
            trackChanges: false
        );
        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(employeeDtos);
    }

    [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        var employee = await service.EmployeeService.GetEmployeeAsync(
            companyId,
            employeeId,
            trackChanges: false
        );
        return Ok(employee);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(
        Guid companyId,
        [FromBody] EmployeeForCreationDto employeeForCreationDto
    )
    {
        var employee = await service.EmployeeService.CreateEmployeeForCompanyAsync(
            companyId,
            employeeForCreationDto,
            trackChanges: false
        );
        return CreatedAtRoute(
            "GetEmployeeForCompany",
            new { companyId, employeeId = employee.Id },
            employee
        );
    }

    [HttpGet("collection/({employeeIds})", Name = "EmployeeCollection")]
    public async Task<IActionResult> GetEmployeeCollection(
        Guid companyId,
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> employeeIds
    )
    {
        var companies = await service.EmployeeService.GetByIdsAsync(
            companyId,
            employeeIds,
            trackChanges: false
        );
        return Ok(companies);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateEmployeeCollection(
        Guid companyId,
        [FromBody] IEnumerable<EmployeeForCreationDto> employeeForCreationDtos
    )
    {
        var (employeeDtos, employeeIds) =
            await service.EmployeeService.CreateEmployeesForCompanyCollectionAsync(
                companyId,
                employeeForCreationDtos
            );
        return CreatedAtRoute("EmployeeCollection", new { companyId, employeeIds }, employeeDtos);
    }

    [HttpDelete("{employeeId:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        await service.EmployeeService.DeleteEmployeeForCompanyAsync(
            companyId,
            employeeId,
            trackChanges: false
        );
        return NoContent();
    }

    [HttpPut("{employeeId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(
        Guid companyId,
        Guid employeeId,
        [FromBody] EmployeeForUpdateDto employeeForUpdateDto
    )
    {
        await service.EmployeeService.UpdateEmployeeForCompanyAsync(
            companyId,
            employeeId,
            employeeForUpdateDto,
            companyTrackChanges: false,
            employeeTrackChanges: true
        );
        return NoContent();
    }

    [HttpPatch("{employeeId:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
        Guid companyId,
        Guid employeeId,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> jsonPatchDocument
    )
    {
        if (jsonPatchDocument is null)
        {
            return BadRequest("patchDoc object sent from client is null.");
        }

        (EmployeeForUpdateDto employeeForUpdateDto, Employee employee) =
            await service.EmployeeService.GetEmployeeForPatchAsync(
                companyId,
                employeeId,
                companyTrackChanges: false,
                employeeTrackChanges: true
            );
        jsonPatchDocument.ApplyTo(employeeForUpdateDto, ModelState);

        TryValidateModel(employeeForUpdateDto);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await service.EmployeeService.SaveChangesForPatchAsync(employeeForUpdateDto, employee);
        return NoContent();
    }
}
