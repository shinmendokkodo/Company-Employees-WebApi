using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Entities.LinkModels;
using Microsoft.AspNetCore.Http;
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
    [HttpHead]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetAll(Guid companyId, [FromQuery] EmployeeParameters employeeParams)
    {
        var employeeLinkParams = new EmployeeLinkParameters(employeeParams, HttpContext);

        var (linkResponse, metadata) = await service.EmployeeService.GetAllAsync(companyId, employeeLinkParams);
        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(metadata));
        
        return linkResponse.HasLinks ? Ok(linkResponse.LinkedEntities) : Ok(linkResponse.ShapedEntities);
    }

    [HttpGet("{employeeId:guid}", Name = "GetById")]
    public async Task<IActionResult> GetById(Guid companyId, Guid employeeId)
    {
        var employee = await service.EmployeeService.GetByIdAsync(companyId, employeeId);
        return Ok(employee);
    }

    [HttpGet("collection/({employeeIds})", Name = "GetByIds")]
    public async Task<IActionResult> GetByIds(Guid companyId, [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> employeeIds)
    {
        var companies = await service.EmployeeService.GetByIdsAsync(companyId, employeeIds, false);
        return Ok(companies);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Create(Guid companyId, [FromBody] EmployeeCreateDto employeeCreateDto)
    {
        var employee = await service.EmployeeService.CreateAsync(companyId, employeeCreateDto);
        return CreatedAtRoute("GetById", new { companyId, employeeId = employee.Id }, employee);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCollection(Guid companyId, [FromBody] IEnumerable<EmployeeCreateDto> employeeCreateDtos)
    {
        var (employeeDtos, employeeIds) =  await service.EmployeeService.CreateCollectionAsync(companyId, employeeCreateDtos);
        return CreatedAtRoute("GetByIds", new { companyId, employeeIds }, employeeDtos);
    }

    [HttpPut("{employeeId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Update(Guid companyId, Guid employeeId, [FromBody] EmployeeUpdateDto employeeUpdateDto)
    {
        await service.EmployeeService.UpdateAsync(companyId, employeeId, employeeUpdateDto);
        return NoContent();
    }

    [HttpPatch("{employeeId:guid}")]
    public async Task<IActionResult> Patch(Guid companyId, Guid employeeId, [FromBody] JsonPatchDocument<EmployeeUpdateDto> jsonPatchDocument)
    {
        if (jsonPatchDocument is null)
        {
            return BadRequest("Patch object sent from client is null.");
        }

        (var employeeUpdateDto, var employee) = await service.EmployeeService.GetByIdPatchAsync(companyId, employeeId);
        jsonPatchDocument.ApplyTo(employeeUpdateDto, ModelState);

        TryValidateModel(employeeUpdateDto);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await service.EmployeeService.SavePatchAsync(employeeUpdateDto, employee);
        return NoContent();
    }

    [HttpDelete("{employeeId:guid}")]
    public async Task<IActionResult> Delete(Guid companyId, Guid employeeId)
    {
        await service.EmployeeService.DeleteAsync(companyId, employeeId);
        return NoContent();
    }

    [HttpOptions]
    public IActionResult GetOptions()
    {
        Response.Headers.Append("Allow", "GET, OPTIONS, POST, PUT, PATCH, DELETE");
        return Ok();
    }
}