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

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManager service) : ControllerBase
{
    [HttpHead]
    [HttpGet(Name = "GetCompanies")]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetAll([FromQuery] CompanyParameters companyParams)
    {
        var companyLinkParams = new CompanyLinkParameters(companyParams, HttpContext);

        var (linkResponse, metadata) = await service.CompanyService.GetAllAsync(companyLinkParams);
        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(metadata));

        return linkResponse.HasLinks ? Ok(linkResponse.LinkedEntities) : Ok(linkResponse.ShapedEntities);
    }

    [HttpGet("{companyId:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetById(Guid companyId)
    {
        var company = await service.CompanyService.GetByIdAsync(companyId);
        return Ok(company);
    }

    [HttpGet("collection/({companyIds})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetByIds([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> companyIds)
    {
        var companies = await service.CompanyService.GetByIdsAsync(companyIds);
        return Ok(companies);
    }

    [HttpPost(Name = "CreateCompany")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Create([FromBody] CompanyCreateDto companyCreateDto)
    {
        var company = await service.CompanyService.CreateAsync(companyCreateDto);
        return CreatedAtRoute("CompanyById", new { companyId = company.Id }, company);
    }    

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCollection([FromBody] IEnumerable<CompanyCreateDto> companyCreateDtos)
    {
        var (companyDtos, companyIds) = await service.CompanyService.CreateCollectionAsync(companyCreateDtos);
        return CreatedAtRoute("CompanyCollection", new { companyIds }, companyDtos);
    }

    [HttpPut("{companyId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Update(Guid companyId, [FromBody] CompanyUpdateDto companyUpdateDto)
    {
        await service.CompanyService.UpdateAsync(companyId, companyUpdateDto);
        return NoContent();
    }

    [HttpPatch("{companyId:guid}")]
    public async Task<IActionResult> Patch(Guid companyId, [FromBody] JsonPatchDocument<CompanyManipulateDto> jsonPatchDocument)
    {
        if (jsonPatchDocument is null)
        {
            return BadRequest("Patch object sent from client is null.");
        }

        var (companyManipulateDto, company) = await service.CompanyService.GetByIdPatchAsync(companyId);
        jsonPatchDocument.ApplyTo(companyManipulateDto, ModelState);

        TryValidateModel(companyManipulateDto);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await service.CompanyService.SavePatchAsync(companyManipulateDto, company);
        return NoContent();
    }

    [HttpDelete("{companyId:guid}")]
    public async Task<IActionResult> Delete(Guid companyId)
    {
        await service.CompanyService.DeleteAsync(companyId);
        return NoContent();
    }

    [HttpOptions]
    public IActionResult GetOptions()
    {
        Response.Headers.Append("Allow", "GET, OPTIONS, POST, PUT, PATCH, DELETE");
        return Ok();
    }
}