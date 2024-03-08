using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utilities.Links;

public class EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper) : BaseLinks<EmployeeDto>(dataShaper), IEmployeeLinks
{
    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeesDto, string? fields, Guid companyId, HttpContext httpContext)
    {
        var shapedEntities = ShapeData(employeesDto, fields);
        return ShouldGenerateLinks(httpContext)
            ? ReturnLinkedEntites(employeesDto, fields, companyId, httpContext, shapedEntities)
            : ReturnShapedEntities(shapedEntities);
    }

    private LinkResponse ReturnLinkedEntites(IEnumerable<EmployeeDto> employeesDto, string? fields, Guid companyId, HttpContext httpContext, List<Entity> shapedEmployees)
    {
        var employeeDtoList = employeesDto.ToList();

        for (var index = 0; index < employeeDtoList.Count; index++)
        {
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
            shapedEmployees[index].Add("Links", employeeLinks);
        }

        LinkCollectionWrapper<Entity> employeeCollection = new(shapedEmployees);
        var linkedEmployees = CreateLinks(httpContext, employeeCollection);

        return new LinkResponse
        {
            HasLinks = true,
            LinkedEntities = linkedEmployees
        };
    }

    protected override LinkCollectionWrapper<Entity> CreateLinks(HttpContext httpContext, LinkCollectionWrapper<Entity> employeesWrapper)
    {
        employeesWrapper.Links.Add(new(linkGenerator.GetUriByAction(httpContext, "GetAll", values: new { }), "self", "GET"));
        return employeesWrapper;
    }

    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid employeeId, string? fields = "")
    {
        List<Link> links =
        [
            new Link(linkGenerator.GetUriByAction(httpContext, "GetById", values: new { companyId, employeeId, fields }), "self", "GET"),
            new Link(linkGenerator.GetUriByAction(httpContext, "Delete", values: new { companyId, employeeId }), "delete_employee", "DELETE"),
            new Link(linkGenerator.GetUriByAction(httpContext, "Update", values: new { companyId, employeeId }), "update_employee", "PUT"),
            new Link(linkGenerator.GetUriByAction(httpContext, "Patch", values: new { companyId, employeeId }), "partially_update_employee", "PATCH")
        ];

        return links;
    }
}