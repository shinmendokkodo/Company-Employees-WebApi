using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeesController(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetEmployeesForCompany(Guid companyId)
    {
        var employees = service.EmployeeService.GetEmployees(companyId, trackChanges: false); 
        return Ok(employees);
    }

    [HttpGet("{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public IActionResult GetEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        var employee = service.EmployeeService.GetEmployee(companyId, employeeId, trackChanges: false); 
        return Ok(employee);
    }

    [HttpPost]
    public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeForCreationDto)
    {
        if (employeeForCreationDto is null)
        {
            return BadRequest("EmployeeForCreationDto object is null");
        }

        var employee = service.EmployeeService.CreateEmployeeForCompany(companyId, employeeForCreationDto, trackChanges: false);
        return CreatedAtRoute("GetEmployeeForCompany", new { companyId, employeeId = employee.Id }, employee);
    }

    [HttpGet("collection/({employeeIds})", Name = "EmployeeCollection")]
    public IActionResult GetEmployeeCollection(Guid companyId, [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> employeeIds)
    {
        var companies = service.EmployeeService.GetByIds(companyId, employeeIds, trackChanges: false);
        return Ok(companies);
    }

    [HttpDelete("{employeeId:guid}")] 
    public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid employeeId) 
    { 
        service.EmployeeService.DeleteEmployeeForCompany(companyId, employeeId, trackChanges: false); 
        return NoContent(); 
    }

    [HttpPut("{employeeId:guid}")] 
    public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employeeForUpdateDto) 
    {
        if (employeeForUpdateDto is null)
        {
            return BadRequest("EmployeeForUpdateDto object is null");
        }

        service.EmployeeService.UpdateEmployeeForCompany(companyId, employeeId, employeeForUpdateDto, companyTrackChanges: false, employeeTrackChanges: true);
        return NoContent(); 
    }
}