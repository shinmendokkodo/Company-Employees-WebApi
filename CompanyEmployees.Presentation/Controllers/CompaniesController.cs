﻿using CompanyEmployees.Presentation.ModelBinders;
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

    [HttpGet("{companyId:guid}", Name = "CompanyById")]
    public IActionResult GetCompany(Guid companyId)
    {
        var company = service.CompanyService.GetCompany(companyId, trackChanges: false); 
        return Ok(company);
    }

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
    {
        if (companyForCreationDto is null) 
            return BadRequest("CompanyForCreationDto object is null"); 

        var company = service.CompanyService.CreateCompany(companyForCreationDto); 
        return CreatedAtRoute("CompanyById", new { companyId = company.Id }, company);
    }

    [HttpGet("collection/({companyIds})", Name = "CompanyCollection")]
    public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> companyIds)
    { 
        var companies = service.CompanyService.GetByIds(companyIds, trackChanges: false); 
        return Ok(companies); 
    }

    [HttpPost("collection")]
    public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyForCreationDtos) 
    {
        var (companyDtos, companyIds) = service.CompanyService.CreateCompanyCollection(companyForCreationDtos);
        return CreatedAtRoute("CompanyCollection", new { companyIds }, companyDtos); 
    }
}