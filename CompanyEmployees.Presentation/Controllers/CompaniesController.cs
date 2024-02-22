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
    public IActionResult GetCompanies()
    {
        var companies = service.CompanyService.GetAllCompanies(trackChanges: false);
        return Ok(companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    public IActionResult GetCompany(Guid id)
    {
        var company = service.CompanyService.GetCompany(id, trackChanges: false); 
        return Ok(company);
    }

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
    {
        if (companyForCreationDto is null) 
            return BadRequest("CompanyForCreationDto object is null"); 

        var company = service.CompanyService.CreateCompany(companyForCreationDto); 
        return CreatedAtRoute("CompanyById", new { id = company.Id }, company);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    { 
        var companies = service.CompanyService.GetByIds(ids, trackChanges: false); 
        return Ok(companies); 
    }

    [HttpPost("collection")]
    public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection) 
    {
        var (companies, ids) = service.CompanyService.CreateCompanyCollection(companyCollection);
        return CreatedAtRoute("CompanyCollection", new { ids }, companies); 
    }
}