using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;
using Xunit;

namespace Library.Tests.Domain.Entities;

public sealed class RequestTests
{
    private static Request CreateBorrowRequest()
        => new(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);

    [Fact]
    public void Constructor_ForBorrowRequest_StartsPending()
    {
        var request = CreateBorrowRequest();

        Assert.Equal(RequestStatus.Pending, request.Status);
    }

    [Fact]
    public void Constructor_ForAddBookRequest_KeepsCopiesCount()
    {
        var request = new Request(
            Guid.NewGuid(), "Anna", Guid.NewGuid(), RequestType.AddBook, 4);

        Assert.Equal(4, request.CopiesCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyRequesterName_ThrowsArgumentException(string requesterName)
    {
        Assert.Throws<ArgumentException>(
            () => new Request(Guid.NewGuid(), requesterName, Guid.NewGuid(), RequestType.BorrowBook, 0));
    }

    [Fact]
    public void Constructor_WithUndefinedType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), (RequestType)999, 0));
    }

    [Fact]
    public void Constructor_ForAddBookRequest_WithoutCopies_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Request(Guid.NewGuid(), "Anna", Guid.NewGuid(), RequestType.AddBook, 0));
    }

    [Fact]
    public void Constructor_ForBorrowRequest_WithCopies_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 2));
    }

    [Fact]
    public void Approve_WhenPending_SetsApproved()
    {
        var request = CreateBorrowRequest();

        request.Approve();

        Assert.Equal(RequestStatus.Approved, request.Status);
    }

    [Fact]
    public void Reject_WhenPending_SetsRejected()
    {
        var request = CreateBorrowRequest();

        request.Reject();

        Assert.Equal(RequestStatus.Rejected, request.Status);
    }

    [Fact]
    public void Approve_WhenAlreadyProcessed_ThrowsRequestAlreadyProcessedException()
    {
        var request = CreateBorrowRequest();
        request.Approve();

        Assert.Throws<RequestAlreadyProcessedException>(() => request.Approve());
    }

    [Fact]
    public void Reject_WhenAlreadyProcessed_ThrowsRequestAlreadyProcessedException()
    {
        var request = CreateBorrowRequest();
        request.Reject();

        Assert.Throws<RequestAlreadyProcessedException>(() => request.Reject());
    }
}
