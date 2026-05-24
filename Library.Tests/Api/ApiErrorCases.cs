using Library.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Library.Tests.Api;

// Maps each HTTP status the controllers produce to a sample exception that triggers it.
public static class ApiErrorCases
{
    public static Exception ForStatus(int statusCode)
        => statusCode switch
        {
            StatusCodes.Status404NotFound => new UserNotFoundException("Ghost"),
            StatusCodes.Status409Conflict => new LibraryException("Conflict."),
            StatusCodes.Status400BadRequest => new ArgumentException("Invalid argument."),
            _ => throw new ArgumentOutOfRangeException(nameof(statusCode)),
        };
}
