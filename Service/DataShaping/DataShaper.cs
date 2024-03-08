using System.Dynamic;
using System.Reflection;
using Contracts;
using Entities.Models;

namespace Service.DataShaping;

public class DataShaper<T> : IDataShaper<T> where T : class
{
    public PropertyInfo[] Properties { get; set; }

    public DataShaper()
    {
        Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }

    public IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string? fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        return FetchData(entities, requiredProperties);
    }

    public ShapedEntity ShapeData(T entity, string? fieldsString)
    {
        var requiredProperties = GetRequiredProperties(fieldsString);
        return FetchData(entity, requiredProperties);
    }

    private IEnumerable<PropertyInfo> GetRequiredProperties(string? fieldsString)
    {
        List<PropertyInfo> requiredProperties = [];

        if (!string.IsNullOrWhiteSpace(fieldsString))
        {
            var fields = fieldsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var field in fields)
            {
                var property = Properties.FirstOrDefault(pi => pi.Name.Equals(field.Trim(), StringComparison.InvariantCultureIgnoreCase));

                if (property == null)
                {
                    continue;
                }

                requiredProperties.Add(property);
            }
        }
        else
        {
            requiredProperties = [.. Properties];
        }

        return requiredProperties.AsEnumerable();
    }

    private static IEnumerable<ShapedEntity> FetchData(IEnumerable<T> entities, IEnumerable<PropertyInfo> properties)
    {
        List<ShapedEntity> shapedData = [];

        foreach (var entity in entities)
        {
            var shapedEntity = DataShaper<T>.FetchData(entity, properties);
            shapedData.Add(shapedEntity);
        }

        return shapedData.AsEnumerable();
    }

    private static ShapedEntity FetchData(T entity, IEnumerable<PropertyInfo> properties)
    {
        ShapedEntity shapedEntity = new();

        foreach (var property in properties)
        {
            var objectPropertyValue = property.GetValue(entity);
            shapedEntity.Entity.TryAdd(property.Name, objectPropertyValue);
        }

        var objectProperty = entity.GetType().GetProperty("Id");
        shapedEntity.Id = objectProperty?.GetValue(entity) is Guid guid ? guid : Guid.Empty;;

        return shapedEntity;
    }
}
