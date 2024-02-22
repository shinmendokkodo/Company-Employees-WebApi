using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
    {
        var employees = await service.EmployeeService.GetEmployees(companyId, trackChanges: false); 
        return Ok(employees);
    }

    [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        var employee = await service.EmployeeService.GetEmployee(companyId, employeeId, trackChanges: false); 
        return Ok(employee);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreationDto)
    {
        var employee = await service.EmployeeService.CreateEmployeeForCompany(companyId, employeeForCreationDto, trackChanges: false);
        return CreatedAtRoute("GetEmployeeForCompany", new { companyId, employeeId = employee.Id }, employee);
    }

    [HttpGet("collection/({employeeIds})", Name = "EmployeeCollection")]
    public async Task<IActionResult> GetEmployeeCollection(Guid companyId, [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> employeeIds)
    {
        var companies = await service.EmployeeService.GetByIds(companyId, employeeIds, trackChanges: false);
        return Ok(companies);
    }

    [HttpDelete("{employeeId:guid}")] 
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId) 
    { 
        await service.EmployeeService.DeleteEmployeeForCompany(companyId, employeeId, trackChanges: false); 
        return NoContent(); 
    }

    [HttpPut("{employeeId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employeeForUpdateDto) 
    {
        await service.EmployeeService.UpdateEmployeeForCompany(companyId, employeeId, employeeForUpdateDto, companyTrackChanges: false, employeeTrackChanges: true);
        return NoContent(); 
    }

    [HttpPatch("{employeeId:guid}")] 
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> jsonPatchDocument) 
    { 
        if (jsonPatchDocument is null)
        {
            return BadRequest("patchDoc object sent from client is null.");
        }

        (EmployeeForUpdateDto employeeForUpdateDto, Employee employee) = await service.EmployeeService.GetEmployeeForPatch(companyId, employeeId, companyTrackChanges: false, employeeTrackChanges: true);
        jsonPatchDocument.ApplyTo(employeeForUpdateDto, ModelState);

        TryValidateModel(employeeForUpdateDto);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await service.EmployeeService.SaveChangesForPatch(employeeForUpdateDto, employee); 
        return NoContent(); 
    }
}