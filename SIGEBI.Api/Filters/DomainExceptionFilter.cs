using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SIGEBI.Domain.Base;

namespace SIGEBI.Api.Filters;

public sealed class DomainExceptionFilter : IExceptionFilter
{
    private readonly ILogger<DomainExceptionFilter> _logger;

    public DomainExceptionFilter(ILogger<DomainExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DomainException domainException)
        {
            return;
        }

        _logger.LogWarning(context.Exception, "Domain validation error: {Message}", context.Exception.Message);

        var problemDetails = new ProblemDetails
        {
            Title = "Se produjo un error de validación en el dominio.",
            Detail = domainException.Message,
            Status = StatusCodes.Status400BadRequest
        };

        if (!string.IsNullOrWhiteSpace(domainException.PropertyName))
        {
            problemDetails.Extensions["propertyName"] = domainException.PropertyName;
        }

        context.Result = new BadRequestObjectResult(problemDetails);
        context.ExceptionHandled = true;
    }
}
