using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Utilities.Links;

public class CompanyLinks(LinkGenerator linkGenerator, IDataShaper<CompanyDto> dataShaper) : BaseLinks<CompanyDto>(dataShaper), ICompanyLinks
{
    public LinkResponse TryGenerateLinks(IEnumerable<CompanyDto> companyDtos, string? fields, HttpContext httpContext)
    {
        var shapedEntities = ShapeData(companyDtos, fields);
        return ShouldGenerateLinks(httpContext)
            ? ReturnLinkedEntities(companyDtos, fields, httpContext, shapedEntities)
            : ReturnShapedEntities(shapedEntities);
    }

    private LinkResponse ReturnLinkedEntities(IEnumerable<CompanyDto> companyDtos, string? fields, HttpContext httpContext, List<Entity> shapedEntities)
    {
        var companyDtoList = companyDtos.ToList();

        for (var index = 0; index < companyDtoList.Count; index++)
        {
            var entityLinks = CreateLinks(httpContext, companyDtoList[index].Id, fields);
            shapedEntities[index].Add("Links", entityLinks);
        }

        LinkCollectionWrapper<Entity> entityCollection = new(shapedEntities);
        var linkedCompanies = CreateLinks(httpContext, entityCollection);

        return new LinkResponse 
        { 
            HasLinks = true, 
            LinkedEntities = linkedCompanies 
        };
    }

    private List<Link> CreateLinks(HttpContext httpContext, Guid companyId, string? fields = "")
    {
        List<Link> links =
        [
            new(linkGenerator.GetUriByAction(httpContext, "GetById", values: new { companyId, fields }), "self", "GET"),
            new(linkGenerator.GetUriByAction(httpContext, "Delete", values: new { companyId }), "delete_company", "DELETE"),
            new(linkGenerator.GetUriByAction(httpContext, "Update", values: new { companyId }), "update_company", "PUT"),
            new(linkGenerator.GetUriByAction(httpContext, "Patch", values: new { companyId }), "partially_update_company", "PATCH")
        ];

        return links;
    }

    protected override LinkCollectionWrapper<Entity> CreateLinks(HttpContext httpContext, LinkCollectionWrapper<Entity> linkCollectionWrapper)
    {
        linkCollectionWrapper.Links.Add(new(linkGenerator.GetUriByAction(httpContext, "GetAll", values: new { }), "self", "GET"));
        return linkCollectionWrapper;
    }
}