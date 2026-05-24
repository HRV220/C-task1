using Library.Api.Contracts.Requests;
using Library.Api.Mapping;
using Library.Application.Abstractions.Services;
using Library.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers;

[ApiController]
[Route("api/books")]
public sealed class BooksController : ControllerBase
{
    private readonly IBookService _books;

    public BooksController(IBookService books)
    {
        _books = books;
    }

    [HttpPost]
    public IActionResult RegisterBook(RegisterBookRequest request)
    {
        try
        {
            var book = _books.RegisterBook(request.WriterName, request.Title, request.Circulation);

            return Created($"/api/books/{book.Id}", book.ToResponse());
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
