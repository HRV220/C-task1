using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Library.Tests.Api;

// Lets ControllerBase.Problem(...) build a ProblemDetails without a real HttpContext / DI container.
public sealed class TestProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
        => new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
        };

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
        => new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode,
        };
}
