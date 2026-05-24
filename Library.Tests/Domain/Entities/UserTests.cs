using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;
using Xunit;

namespace Library.Tests.Domain.Entities;

public sealed class UserTests
{
    [Fact]
    public void Constructor_WithValidArguments_CreatesUserWithoutBorrowedBooks()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });

        Assert.Equal("Ivan", user.Name);
        Assert.Contains(UserRole.Reader, user.Roles);
        Assert.Empty(user.BorrowedBooks);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithEmptyName_ThrowsArgumentException(string name)
    {
        Assert.Throws<ArgumentException>(() => new User(name, new[] { UserRole.Reader }));
    }

    [Fact]
    public void Constructor_WithEmptyRoles_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new User("Ivan", Array.Empty<UserRole>()));
    }

    [Fact]
    public void HasRole_WhenRoleIsAssigned_ReturnsTrueOnlyForAssignedRoles()
    {
        var user = new User("Ivan", new[] { UserRole.Reader, UserRole.Librarian });

        Assert.True(user.HasRole(UserRole.Librarian));
        Assert.False(user.HasRole(UserRole.Writer));
    }

    [Fact]
    public void BorrowBook_WhenNotBorrowed_AddsBookToBorrowed()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });
        var bookId = Guid.NewGuid();

        user.BorrowBook(bookId);

        Assert.True(user.HasBorrowed(bookId));
    }

    [Fact]
    public void BorrowBook_WhenAlreadyBorrowed_ThrowsBookAlreadyBorrowedException()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });
        var bookId = Guid.NewGuid();
        user.BorrowBook(bookId);

        Assert.Throws<BookAlreadyBorrowedException>(() => user.BorrowBook(bookId));
    }

    [Fact]
    public void ReturnBook_WhenBorrowed_RemovesBookFromBorrowed()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });
        var bookId = Guid.NewGuid();
        user.BorrowBook(bookId);

        user.ReturnBook(bookId);

        Assert.False(user.HasBorrowed(bookId));
    }

    [Fact]
    public void ReturnBook_WhenNotBorrowed_ThrowsBookNotBorrowedException()
    {
        var user = new User("Ivan", new[] { UserRole.Reader });
        var bookId = Guid.NewGuid();

        Assert.Throws<BookNotBorrowedException>(() => user.ReturnBook(bookId));
    }
}
