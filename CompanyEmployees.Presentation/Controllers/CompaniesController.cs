using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await service.CompanyService.GetAllCompanies(trackChanges: false);
        return Ok(companies);
    }

    [HttpGet("{companyId:guid}", Name = "CompanyById")]
    public async Task<IActionResult> GetCompany(Guid companyId)
    {
        var company = await service.CompanyService.GetCompany(companyId, trackChanges: false); 
        return Ok(company);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
    {
        if (companyForCreationDto is null)
        {
            return BadRequest("CompanyForCreationDto object is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        var company = await service.CompanyService.CreateCompany(companyForCreationDto); 
        return CreatedAtRoute("CompanyById", new { companyId = company.Id }, company);
    }

    [HttpGet("collection/({companyIds})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> companyIds)
    { 
        var companies = await service.CompanyService.GetByIds(companyIds, trackChanges: false); 
        return Ok(companies); 
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyForCreationDtos) 
    {
        var (companyDtos, companyIds) = await service.CompanyService.CreateCompanyCollection(companyForCreationDtos);
        return CreatedAtRoute("CompanyCollection", new { companyIds }, companyDtos); 
    }

    [HttpDelete("{companyId:guid}")] 
    public async Task<IActionResult> DeleteCompany(Guid companyId) 
    {
        await service.CompanyService.DeleteCompany(companyId, trackChanges: false); 
        return NoContent(); 
    }

    [HttpPut("{companyId:guid}")] 
    public async Task<IActionResult> UpdateCompany(Guid companyId, [FromBody] CompanyForUpdateDto companyForUpdateDto) 
    { 
        if (companyForUpdateDto is null)
        {
            return BadRequest("CompanyForUpdateDto object is null");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await service.CompanyService.UpdateCompany(companyId, companyForUpdateDto, trackChanges: true); 
        return NoContent(); 
    }
}