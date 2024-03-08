using System.Linq.Dynamic.Core;
using Entities.Models;
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

        var lowerCaseTerm = searchTerm.Trim().ToLower();

        return companies.Where(c => c.Name != null && c.Name.ToLower().Contains(searchTerm));
    }

    public static IQueryable<Company> Sort(this IQueryable<Company> companies, string? orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
        {
            return companies.OrderBy(c => c.Name);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Company>(orderBy);

        return string.IsNullOrWhiteSpace(orderQuery)
            ? (IQueryable<Company>)companies.OrderBy(c => c.Name)
            : companies.OrderBy(orderQuery);
    }
}