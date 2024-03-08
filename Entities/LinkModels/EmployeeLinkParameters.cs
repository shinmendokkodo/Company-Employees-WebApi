using Microsoft.AspNetCore.Http;
using Shared.RequestFeatures;

namespace Entities.LinkModels
{
    public record EmployeeLinkParameters(EmployeeParameters EmployeeParameters, HttpContext Context);
}