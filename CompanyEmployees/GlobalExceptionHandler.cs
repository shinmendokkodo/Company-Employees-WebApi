using System.Net;
using System.Net.Mime;
using Contracts;
using Entities.ErrorModel;
using Microsoft.AspNetCore.Diagnostics;

namespace CompanyEmployees;

public class GlobalExceptionHandler(ILoggerManager logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        
        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature == null) return true;
        
        logger.LogError($"Something went wrong: {exception.Message}"); 
        await httpContext.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = httpContext.Response.StatusCode, 
            Message = "Internal Server Error.",
        }.ToString(), cancellationToken: cancellationToken);

        return true;
    }
}