using Library.Api.Contracts.Requests;
using Library.Api.Mapping;
using Library.Application.Abstractions.Services;
using Library.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/requests")]
public sealed class RequestsController : ControllerBase
{
    private readonly IRequestService _requests;

    public RequestsController(IRequestService requests)
    {
        _requests = requests;
    }

    [HttpPost("borrow/{bookId:guid}")]
    public IActionResult CreateBorrowRequest(Guid bookId, ReaderRequest request)
    {
        try
        {
            var created = _requests.CreateBorrowRequest(request.ReaderName, bookId);

            return Created($"/api/requests/{created.Id}", created.ToResponse());
        }
        catch (NotFoundException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (LibraryException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (ArgumentException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("return/{bookId:guid}")]
    public IActionResult CreateReturnRequest(Guid bookId, ReaderRequest request)
    {
        try
        {
            var created = _requests.CreateReturnRequest(request.ReaderName, bookId);

            return Created($"/api/requests/{created.Id}", created.ToResponse());
        }
        catch (NotFoundException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (LibraryException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (ArgumentException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("add-book/{bookId:guid}")]
    public IActionResult CreateAddBookRequest(Guid bookId, AddBookCopiesRequest request)
    {
        try
        {
            var created = _requests.CreateAddBookRequest(request.WriterName, bookId, request.CopiesCount);

            return Created($"/api/requests/{created.Id}", created.ToResponse());
        }
        catch (NotFoundException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (LibraryException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (ArgumentException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{requestId:guid}/approve")]
    public IActionResult Approve(Guid requestId, LibrarianDecisionRequest request)
    {
        try
        {
            var approved = _requests.Approve(requestId, request.LibrarianName);

            return Ok(approved.ToResponse());
        }
        catch (NotFoundException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (LibraryException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (ArgumentException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }

    [HttpPost("{requestId:guid}/reject")]
    public IActionResult Reject(Guid requestId, LibrarianDecisionRequest request)
    {
        try
        {
            var rejected = _requests.Reject(requestId, request.LibrarianName);

            return Ok(rejected.ToResponse());
        }
        catch (NotFoundException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (LibraryException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (ArgumentException exception)
        {
            return Problem(exception.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
