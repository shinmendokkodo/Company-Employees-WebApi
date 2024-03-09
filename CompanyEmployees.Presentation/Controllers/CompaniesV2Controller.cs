using System.Text.Json;
using Asp.Versioning;
using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.RequestFeatures;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesV2Controller(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParams)
    {
        var companyLinkParams = new CompanyLinkParameters(companyParams, HttpContext);

        var (linkResponse, metadata) = await service.CompanyService.GetAllAsync(companyLinkParams);
        Response.Headers.TryAdd("X-Pagination", JsonSerializer.Serialize(metadata));

        return linkResponse.HasLinks ? Ok(linkResponse.LinkedEntities) : Ok(linkResponse.ShapedEntities);
    }
}