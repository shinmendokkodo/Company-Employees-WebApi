using System.Net.Mime;
using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace CompanyEmployees.Utilities.ExceptionHandler;

public class GlobalExceptionHandler(ILoggerManager logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature == null)
        {
            return true;
        }

        httpContext.Response.StatusCode = contextFeature.Error switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            BadRequestException => StatusCodes.Status400BadRequest,
            UnprocessableEntityException => StatusCodes.Status422UnprocessableEntity,
            _ => StatusCodes.Status500InternalServerError
        };

        logger.LogError($"Something went wrong: {exception.Message}");

        await httpContext.Response.WriteAsync
        (
            new ErrorDetails 
            { 
                StatusCode = httpContext.Response.StatusCode, 
                Message = contextFeature.Error.Message 
            }.ToString(), 
            cancellationToken
        );

        return true;
    }
}