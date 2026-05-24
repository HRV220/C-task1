using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Infrastructure.Repositories;
using Xunit;

namespace Library.Tests.Infrastructure;

public sealed class UserRepositoryTests : SqliteContextTestBase
{
    [Fact]
    public void Add_ThenFindByName_RoundTripsRolesAndBorrowedBooks()
    {
        var bookId = Guid.NewGuid();
        var user = new User("Ivan", new[] { UserRole.Reader, UserRole.Librarian });
        user.BorrowBook(bookId);

        using (var context = CreateContext())
        {
            new UserRepository(context).Add(user);
        }

        using var readContext = CreateContext();
        var loaded = new UserRepository(readContext).FindByName("Ivan");

        Assert.NotNull(loaded);
        Assert.Equal("Ivan", loaded!.Name);
        Assert.Contains(UserRole.Reader, loaded.Roles);
        Assert.Contains(UserRole.Librarian, loaded.Roles);
        Assert.Contains(bookId, loaded.BorrowedBooks);
    }

    [Fact]
    public void FindByName_WhenMissing_ReturnsNull()
    {
        using var context = CreateContext();
        var repository = new UserRepository(context);

        Assert.Null(repository.FindByName("Ghost"));
    }

    [Fact]
    public void Update_PersistsBorrowedBooksChange()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });
        using (var context = CreateContext())
        {
            new UserRepository(context).Add(user);
        }

        var bookId = Guid.NewGuid();
        using (var context = CreateContext())
        {
            var repository = new UserRepository(context);
            var loaded = repository.FindByName("Ivan")!;
            loaded.BorrowBook(bookId);
            repository.Update(loaded);
        }

        using var readContext = CreateContext();
        var result = new UserRepository(readContext).FindByName("Ivan")!;

        Assert.Contains(bookId, result.BorrowedBooks);
    }
}
