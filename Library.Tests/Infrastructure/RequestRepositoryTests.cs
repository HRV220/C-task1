using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Infrastructure.Repositories;
using Xunit;

namespace Library.Tests.Infrastructure;

public sealed class RequestRepositoryTests : SqliteContextTestBase
{
    [Fact]
    public void Add_ThenFindById_RoundTripsRequest()
    {
        var request = new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.AddBook, 3);

        using (var context = CreateContext())
        {
            new RequestRepository(context).Add(request);
        }

        using var readContext = CreateContext();
        var loaded = new RequestRepository(readContext).FindById(request.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Ivan", loaded!.RequesterName);
        Assert.Equal(RequestType.AddBook, loaded.Type);
        Assert.Equal(3, loaded.CopiesCount);
        Assert.Equal(RequestStatus.Pending, loaded.Status);
    }

    [Fact]
    public void GetPending_ReturnsOnlyPendingRequests()
    {
        var pending = new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        var processed = new Request(Guid.NewGuid(), "Anna", Guid.NewGuid(), RequestType.BorrowBook, 0);
        processed.Reject();

        using (var context = CreateContext())
        {
            var repository = new RequestRepository(context);
            repository.Add(pending);
            repository.Add(processed);
        }

        using var readContext = CreateContext();
        var result = new RequestRepository(readContext).GetPending();

        Assert.Single(result);
        Assert.Equal(RequestStatus.Pending, result.First().Status);
    }

    [Fact]
    public void Update_PersistsStatusChange()
    {
        var request = new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        using (var context = CreateContext())
        {
            new RequestRepository(context).Add(request);
        }

        using (var context = CreateContext())
        {
            var repository = new RequestRepository(context);
            var loaded = repository.FindById(request.Id)!;
            loaded.Approve();
            repository.Update(loaded);
        }

        using var readContext = CreateContext();
        var result = new RequestRepository(readContext).FindById(request.Id)!;

        Assert.Equal(RequestStatus.Approved, result.Status);
    }
}
