using Library.Api.Contracts.Requests;
using Library.Api.Contracts.Responses;
using Library.Api.Controllers;
using Library.Application.Abstractions.Services;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests.Api;

public sealed class BooksControllerTests
{
    private readonly Mock<IBookService> _books = new();

    private BooksController CreateController()
        => new(_books.Object) { ProblemDetailsFactory = new TestProblemDetailsFactory() };

    [Fact]
    public void RegisterBook_ReturnsCreatedWithMappedBook()
    {
        var book = new Book(Guid.NewGuid(), "Clean Code", "Anna", 5);
        _books.Setup(service => service.RegisterBook("Anna", "Clean Code", 5)).Returns(book);
        var controller = CreateController();

        var result = controller.RegisterBook(new RegisterBookRequest("Anna", "Clean Code", 5));

        var created = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<BookResponse>(created.Value);
        Assert.Equal(book.Id, response.Id);
        Assert.Equal("Clean Code", response.Title);
        Assert.Equal("Anna", response.AuthorName);
    }

    [Theory]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status409Conflict)]
    [InlineData(StatusCodes.Status400BadRequest)]
    public void RegisterBook_WhenServiceThrows_MapsExceptionToStatus(int statusCode)
    {
        _books
            .Setup(service => service.RegisterBook(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Throws(ApiErrorCases.ForStatus(statusCode));
        var controller = CreateController();

        var result = controller.RegisterBook(new RegisterBookRequest("Anna", "Clean Code", 5));

        var problem = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, problem.StatusCode);
    }
}
