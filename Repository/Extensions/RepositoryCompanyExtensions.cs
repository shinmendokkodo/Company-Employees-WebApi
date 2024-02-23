using System.Linq.Dynamic.Core;
using Entities;
using Repository.Extensions.Utilities;

namespace Repository;

public static class RepositoryCompanyExtensions
{
    public static IQueryable<Company> Search(this IQueryable<Company> companies, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return companies;
        }

        return companies.Where(c =>
            c.Name != null
            && c.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    public static IQueryable<Company> Sort(
        this IQueryable<Company> companies,
        string? orderByQueryString
    )
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return companies.OrderBy(c => c.Name);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Company>(orderByQueryString);

        return string.IsNullOrWhiteSpace(orderQuery)
            ? (IQueryable<Company>)companies.OrderBy(c => c.Name)
            : companies.OrderBy(orderQuery);
    }
}
