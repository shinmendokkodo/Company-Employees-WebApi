using Contracts;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;

namespace CompanyEmployees.Utilities.Links
{
    public abstract class BaseLinks<T>(IDataShaper<T> dataShaper) where T : class
    {
        protected Dictionary<string, MediaTypeHeaderValue> AcceptHeader { get; set; } = [];

        protected bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = httpContext.Items["AcceptHeaderMediaType"] as MediaTypeHeaderValue;
            return mediaType?.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        protected LinkResponse ReturnShapedEntities(List<Entity> shapedCompanies)
        {
            return new() { ShapedEntities = shapedCompanies };
        }

        protected List<Entity> ShapeData(IEnumerable<T> dtos, string? fields)
        {
            return dataShaper.ShapeData(dtos, fields).Select(se => se.Entity).ToList();
        }

        protected abstract LinkCollectionWrapper<Entity> CreateLinks(HttpContext httpContext, LinkCollectionWrapper<Entity> linkCollectionWrapper);
    }
}
