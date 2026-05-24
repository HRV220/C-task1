using Library.Api.Contracts.Requests;
using Library.Api.Contracts.Responses;
using Library.Api.Controllers;
using Library.Application.Abstractions.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests.Api;

public sealed class RequestsControllerTests
{
    private readonly Mock<IRequestService> _requests = new();

    private RequestsController CreateController()
        => new(_requests.Object) { ProblemDetailsFactory = new TestProblemDetailsFactory() };

    [Fact]
    public void CreateBorrowRequest_ReturnsCreatedWithMappedRequest()
    {
        var bookId = Guid.NewGuid();
        var request = new Request(Guid.NewGuid(), "Ivan", bookId, RequestType.BorrowBook, 0);
        _requests.Setup(service => service.CreateBorrowRequest("Ivan", bookId)).Returns(request);
        var controller = CreateController();

        var result = controller.CreateBorrowRequest(bookId, new ReaderRequest("Ivan"));

        var created = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<RequestResponse>(created.Value);
        Assert.Equal("Ivan", response.RequesterName);
        Assert.Equal("BorrowBook", response.Type);
    }

    [Fact]
    public void CreateReturnRequest_ReturnsCreatedWithMappedRequest()
    {
        var bookId = Guid.NewGuid();
        var request = new Request(Guid.NewGuid(), "Ivan", bookId, RequestType.ReturnBook, 0);
        _requests.Setup(service => service.CreateReturnRequest("Ivan", bookId)).Returns(request);
        var controller = CreateController();

        var result = controller.CreateReturnRequest(bookId, new ReaderRequest("Ivan"));

        var created = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<RequestResponse>(created.Value);
        Assert.Equal("ReturnBook", response.Type);
    }

    [Fact]
    public void CreateAddBookRequest_ReturnsCreatedWithMappedRequest()
    {
        var bookId = Guid.NewGuid();
        var request = new Request(Guid.NewGuid(), "Anna", bookId, RequestType.AddBook, 3);
        _requests.Setup(service => service.CreateAddBookRequest("Anna", bookId, 3)).Returns(request);
        var controller = CreateController();

        var result = controller.CreateAddBookRequest(bookId, new AddBookCopiesRequest("Anna", 3));

        var created = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<RequestResponse>(created.Value);
        Assert.Equal("AddBook", response.Type);
        Assert.Equal(3, response.CopiesCount);
    }

    [Fact]
    public void Approve_ReturnsOkWithApprovedRequest()
    {
        var requestId = Guid.NewGuid();
        var request = new Request(requestId, "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        request.Approve();
        _requests.Setup(service => service.Approve(requestId, "Boss")).Returns(request);
        var controller = CreateController();

        var result = controller.Approve(requestId, new LibrarianDecisionRequest("Boss"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<RequestResponse>(ok.Value);
        Assert.Equal("Approved", response.Status);
    }

    [Fact]
    public void Reject_ReturnsOkWithRejectedRequest()
    {
        var requestId = Guid.NewGuid();
        var request = new Request(requestId, "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        request.Reject();
        _requests.Setup(service => service.Reject(requestId, "Boss")).Returns(request);
        var controller = CreateController();

        var result = controller.Reject(requestId, new LibrarianDecisionRequest("Boss"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<RequestResponse>(ok.Value);
        Assert.Equal("Rejected", response.Status);
    }

    [Theory]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status409Conflict)]
    [InlineData(StatusCodes.Status400BadRequest)]
    public void CreateBorrowRequest_WhenServiceThrows_MapsExceptionToStatus(int statusCode)
    {
        _requests
            .Setup(service => service.CreateBorrowRequest(It.IsAny<string>(), It.IsAny<Guid>()))
            .Throws(ApiErrorCases.ForStatus(statusCode));
        var controller = CreateController();

        var result = controller.CreateBorrowRequest(Guid.NewGuid(), new ReaderRequest("Ivan"));

        Assert.Equal(statusCode, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Theory]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status409Conflict)]
    [InlineData(StatusCodes.Status400BadRequest)]
    public void CreateReturnRequest_WhenServiceThrows_MapsExceptionToStatus(int statusCode)
    {
        _requests
            .Setup(service => service.CreateReturnRequest(It.IsAny<string>(), It.IsAny<Guid>()))
            .Throws(ApiErrorCases.ForStatus(statusCode));
        var controller = CreateController();

        var result = controller.CreateReturnRequest(Guid.NewGuid(), new ReaderRequest("Ivan"));

        Assert.Equal(statusCode, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Theory]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status409Conflict)]
    [InlineData(StatusCodes.Status400BadRequest)]
    public void CreateAddBookRequest_WhenServiceThrows_MapsExceptionToStatus(int statusCode)
    {
        _requests
            .Setup(service => service.CreateAddBookRequest(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>()))
            .Throws(ApiErrorCases.ForStatus(statusCode));
        var controller = CreateController();

        var result = controller.CreateAddBookRequest(Guid.NewGuid(), new AddBookCopiesRequest("Anna", 3));

        Assert.Equal(statusCode, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Theory]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status409Conflict)]
    [InlineData(StatusCodes.Status400BadRequest)]
    public void Approve_WhenServiceThrows_MapsExceptionToStatus(int statusCode)
    {
        _requests
            .Setup(service => service.Approve(It.IsAny<Guid>(), It.IsAny<string>()))
            .Throws(ApiErrorCases.ForStatus(statusCode));
        var controller = CreateController();

        var result = controller.Approve(Guid.NewGuid(), new LibrarianDecisionRequest("Boss"));

        Assert.Equal(statusCode, Assert.IsType<ObjectResult>(result).StatusCode);
    }

    [Theory]
    [InlineData(StatusCodes.Status404NotFound)]
    [InlineData(StatusCodes.Status409Conflict)]
    [InlineData(StatusCodes.Status400BadRequest)]
    public void Reject_WhenServiceThrows_MapsExceptionToStatus(int statusCode)
    {
        _requests
            .Setup(service => service.Reject(It.IsAny<Guid>(), It.IsAny<string>()))
            .Throws(ApiErrorCases.ForStatus(statusCode));
        var controller = CreateController();

        var result = controller.Reject(Guid.NewGuid(), new LibrarianDecisionRequest("Boss"));

        Assert.Equal(statusCode, Assert.IsType<ObjectResult>(result).StatusCode);
    }
}
