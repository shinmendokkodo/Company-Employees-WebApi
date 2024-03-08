using System.Net.Mime;
using System.Text;
using Entities.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace CompanyEmployees.Formatters.Output;

public class CsvOutputFormatter : TextOutputFormatter
{
    public CsvOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse(MediaTypeNames.Text.Csv));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var response = context.HttpContext.Response;
        var buffer = new StringBuilder();

        if (context.Object is IEnumerable<Entity> objects)
        {
            var count = objects.Count();
            for (var i = 0; i < count; i++)
            {
                FormatCsvObject(buffer, objects.ElementAt(i));
                if (i < count - 1)
                {
                    buffer.AppendLine(); // Add newline except for the last line
                }
            }
        }
        else if (context.Object is Entity entity)
        {
            FormatCsvObject(buffer, entity);
        }

        await response.WriteAsync(buffer.ToString());
    }

    protected override bool CanWriteType(Type? type)
    {
        if (typeof(Entity).IsAssignableFrom(type) || typeof(IEnumerable<Entity>).IsAssignableFrom(type))
        {
            return base.CanWriteType(type);
        }
        return false;
    }

    private void FormatCsvObject(StringBuilder buffer, Entity entity)
    {
        var values = entity.Values.Select(value => $"\"{value}\"").ToList();
        buffer.Append(string.Join(", ", values));
    }
}