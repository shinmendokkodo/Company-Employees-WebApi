using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;

namespace Entities.LinkModels
{
    public record CompanyLinkParameters(CompanyParameters CompanyParameters, HttpContext Context);
}