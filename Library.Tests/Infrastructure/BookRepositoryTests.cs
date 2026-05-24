using Library.Domain.Entities;
using Library.Infrastructure.Repositories;
using Xunit;

namespace Library.Tests.Infrastructure;

public sealed class BookRepositoryTests : SqliteContextTestBase
{
    [Fact]
    public void Add_ThenFindById_RoundTripsBookWithInventory()
    {
        var book = new Book(Guid.NewGuid(), "Clean Code", "Robert", 5);
        book.AddCopies(3);

        using (var context = CreateContext())
        {
            new BookRepository(context).Add(book);
        }

        using var readContext = CreateContext();
        var loaded = new BookRepository(readContext).FindById(book.Id);

        Assert.NotNull(loaded);
        Assert.Equal("Clean Code", loaded!.Title);
        Assert.Equal("Robert", loaded.AuthorName);
        Assert.Equal(5, loaded.Circulation);
        Assert.Equal(3, loaded.CopiesInLibrary);
        Assert.Equal(3, loaded.AvailableCopies);
    }

    [Fact]
    public void FindByAuthor_ReturnsOnlyAuthorsBooks()
    {
        using (var context = CreateContext())
        {
            var repository = new BookRepository(context);
            repository.Add(new Book(Guid.NewGuid(), "Book A", "Anna", 5));
            repository.Add(new Book(Guid.NewGuid(), "Book B", "Anna", 5));
            repository.Add(new Book(Guid.NewGuid(), "Book C", "Robert", 5));
        }

        using var readContext = CreateContext();
        var annasBooks = new BookRepository(readContext).FindByAuthor("Anna");

        Assert.Equal(2, annasBooks.Count);
    }

    [Fact]
    public void Update_PersistsCopiesChange()
    {
        var book = new Book(Guid.NewGuid(), "Clean Code", "Robert", 5);
        using (var context = CreateContext())
        {
            new BookRepository(context).Add(book);
        }

        using (var context = CreateContext())
        {
            var repository = new BookRepository(context);
            var loaded = repository.FindById(book.Id)!;
            loaded.AddCopies(2);
            repository.Update(loaded);
        }

        using var readContext = CreateContext();
        var result = new BookRepository(readContext).FindById(book.Id)!;

        Assert.Equal(2, result.CopiesInLibrary);
    }
}
