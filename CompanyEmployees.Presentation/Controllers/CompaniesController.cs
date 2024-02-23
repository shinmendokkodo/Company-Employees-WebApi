using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
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
    [HttpGet]
    public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
    {
        var (companyDtos, metaData) = await service.CompanyService.GetAllCompaniesAsync(
            companyParameters,
            trackChanges: false
        );
        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(companyDtos);
    }

    [HttpGet("{companyId:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetCompany(Guid companyId)
    {
        var company = await service.CompanyService.GetCompanyAsync(companyId, trackChanges: false);
        return Ok(company);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany(
        [FromBody] CompanyForCreationDto companyForCreationDto
    )
    {
        var company = await service.CompanyService.CreateCompanyAsync(companyForCreationDto);
        return CreatedAtRoute("CompanyById", new { companyId = company.Id }, company);
    }

    [HttpGet("collection/({companyIds})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> companyIds
    )
    {
        var companies = await service.CompanyService.GetByIdsAsync(companyIds, trackChanges: false);
        return Ok(companies);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection(
        [FromBody] IEnumerable<CompanyForCreationDto> companyForCreationDtos
    )
    {
        var (companyDtos, companyIds) = await service.CompanyService.CreateCompanyCollectionAsync(
            companyForCreationDtos
        );
        return CreatedAtRoute("CompanyCollection", new { companyIds }, companyDtos);
    }

    [HttpDelete("{companyId:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid companyId)
    {
        await service.CompanyService.DeleteCompanyAsync(companyId, trackChanges: false);
        return NoContent();
    }

    [HttpPut("{companyId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(
        Guid companyId,
        [FromBody] CompanyForUpdateDto companyForUpdateDto
    )
    {
        await service.CompanyService.UpdateCompanyAsync(
            companyId,
            companyForUpdateDto,
            trackChanges: true
        );
        return NoContent();
    }

    [HttpPatch("{companyId:guid}")]
    public async Task<IActionResult> PartiallyUpdateCompany(
        Guid companyId,
        [FromBody] JsonPatchDocument<CompanyForManipulationDto> jsonPatchDocument
    )
    {
        if (jsonPatchDocument is null)
        {
            return BadRequest("patchDoc object sent from client is null.");
        }

        var (companyForManipulationDto, company) =
            await service.CompanyService.GetCompanyForPatchAsync(companyId, trackChanges: true);
        jsonPatchDocument.ApplyTo(companyForManipulationDto, ModelState);

        TryValidateModel(companyForManipulationDto);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await service.CompanyService.SaveChangesForPatchAsync(companyForManipulationDto, company);
        return NoContent();
    }
}
