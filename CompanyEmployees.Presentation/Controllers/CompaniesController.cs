using Microsoft.AspNetCore.Mvc;
using Service.Contracts;

namespace CompanyEmployees.Presentation.Controllers;

[Route("api/companies")]
[ApiController]
public class CompaniesController(IServiceManager service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetCompanies()
    {
        throw new Exception("Exception");
        var companies = service.CompanyService.GetAllCompanies(trackChanges: false);
        return Ok(companies);
    }
}