using Library.Domain.Entities;
using Library.Domain.Exceptions;
using Xunit;

namespace Library.Tests.Domain.Entities;

public sealed class BookTests
{
    private static Book CreateBook(int circulation = 5)
        => new(Guid.NewGuid(), "Clean Code", "Robert Martin", circulation);

    [Fact]
    public void Constructor_WithValidArguments_CreatesEmptyInventory()
    {
        var book = CreateBook();

        Assert.Equal(0, book.CopiesInLibrary);
        Assert.Equal(0, book.AvailableCopies);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyTitle_ThrowsArgumentException(string title)
    {
        Assert.Throws<ArgumentException>(
            () => new Book(Guid.NewGuid(), title, "Robert Martin", 5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyAuthorName_ThrowsArgumentException(string authorName)
    {
        Assert.Throws<ArgumentException>(
            () => new Book(Guid.NewGuid(), "Clean Code", authorName, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_WithNonPositiveCirculation_ThrowsArgumentException(int circulation)
    {
        Assert.Throws<ArgumentException>(
            () => new Book(Guid.NewGuid(), "Clean Code", "Robert Martin", circulation));
    }

    [Fact]
    public void AddCopies_WithinCirculation_IncreasesInventory()
    {
        var book = CreateBook(circulation: 5);

        book.AddCopies(3);

        Assert.Equal(3, book.CopiesInLibrary);
        Assert.Equal(3, book.AvailableCopies);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddCopies_WithNonPositiveCount_ThrowsArgumentException(int count)
    {
        var book = CreateBook();

        Assert.Throws<ArgumentException>(() => book.AddCopies(count));
    }

    [Fact]
    public void AddCopies_BeyondCirculation_ThrowsCirculationExceededException()
    {
        var book = CreateBook(circulation: 5);

        Assert.Throws<CirculationExceededException>(() => book.AddCopies(6));
    }

    [Fact]
    public void Borrow_WhenCopyAvailable_DecreasesAvailableCopies()
    {
        var book = CreateBook(circulation: 5);
        book.AddCopies(2);

        book.Borrow();

        Assert.Equal(2, book.CopiesInLibrary);
        Assert.Equal(1, book.AvailableCopies);
    }

    [Fact]
    public void Borrow_WhenNoCopyAvailable_ThrowsNoAvailableCopiesException()
    {
        var book = CreateBook();

        Assert.Throws<NoAvailableCopiesException>(() => book.Borrow());
    }

    [Fact]
    public void Return_AfterBorrow_RestoresAvailableCopies()
    {
        var book = CreateBook(circulation: 5);
        book.AddCopies(2);
        book.Borrow();

        book.Return();

        Assert.Equal(2, book.AvailableCopies);
    }

    [Fact]
    public void Return_WhenAllCopiesAreInLibrary_ThrowsInvalidOperationException()
    {
        var book = CreateBook(circulation: 5);
        book.AddCopies(2);

        Assert.Throws<InvalidOperationException>(() => book.Return());
    }
}
