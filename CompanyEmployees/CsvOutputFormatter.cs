using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;
using System.Text;

namespace CompanyEmployees;

public class CsvOutputFormatter<T> : TextOutputFormatter where T : BaseDto
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

        if (context.Object is IEnumerable<T> objects)
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
        else if (context.Object is T singleObject)
        {
            FormatCsvObject(buffer, singleObject);
        }

        await response.WriteAsync(buffer.ToString());
    }

    protected override bool CanWriteType(Type? type)
    {
        if (typeof(T).IsAssignableFrom(type) || typeof(IEnumerable<T>).IsAssignableFrom(type))
        {
            return base.CanWriteType(type);
        }
        return false;
    }

    private void FormatCsvObject(StringBuilder buffer, T obj)
    {
        buffer.Append(obj.ToCsvString());
    }
}