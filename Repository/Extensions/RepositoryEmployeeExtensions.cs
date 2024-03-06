using System.Linq.Dynamic.Core;
using Entities;
using Repository.Extensions.Utilities;

namespace Repository;

public static class RepositoryEmployeeExtensions
{
    public static IQueryable<Employee> Filter(
        this IQueryable<Employee> employees,
        uint minAge,
        uint maxAge
    ) => employees.Where(e => e.Age >= minAge && e.Age <= maxAge);

    public static IQueryable<Employee> Search(
        this IQueryable<Employee> employees,
        string? searchTerm
    )
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return employees;
        }

        return employees.Where(e =>
            e.Name != null
            && e.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase)
        );
    }

    public static IQueryable<Employee> Sort(
        this IQueryable<Employee> employees,
        string? orderByQueryString
    )
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return employees.OrderBy(e => e.Name);
        }

        var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

        return string.IsNullOrWhiteSpace(orderQuery)
            ? (IQueryable<Employee>)employees.OrderBy(e => e.Name)
            : employees.OrderBy(orderQuery);
    }
}
