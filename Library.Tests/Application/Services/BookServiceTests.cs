using Library.Application.Abstractions.Repositories;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;
using Moq;
using Xunit;

namespace Library.Tests.Application.Services;

public sealed class BookServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IBookRepository> _books = new();

    private BookService CreateService()
        => new(_users.Object, _books.Object);

    [Fact]
    public void RegisterBook_ByWriter_AddsBookWithWriterAsAuthor()
    {
        var writer = new User("Anna", new[] { UserRole.Writer });
        _users.Setup(r => r.FindByName("Anna")).Returns(writer);
        _books.Setup(r => r.Add(It.IsAny<Book>())).Returns<Book>(book => book);
        var service = CreateService();

        var book = service.RegisterBook("Anna", "Clean Code", 5);

        Assert.Equal("Anna", book.AuthorName);
        Assert.Equal("Clean Code", book.Title);
        Assert.Equal(5, book.Circulation);
        _books.Verify(r => r.Add(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public void RegisterBook_WhenWriterMissing_ThrowsUserNotFoundException()
    {
        _users.Setup(r => r.FindByName("Ghost")).Returns((User?)null);
        var service = CreateService();

        Assert.Throws<UserNotFoundException>(() => service.RegisterBook("Ghost", "Clean Code", 5));
    }

    [Fact]
    public void RegisterBook_WhenUserIsNotWriter_ThrowsMissingRoleException()
    {
        var reader = new User("Ivan", new[] { UserRole.Reader });
        _users.Setup(r => r.FindByName("Ivan")).Returns(reader);
        var service = CreateService();

        Assert.Throws<MissingRoleException>(() => service.RegisterBook("Ivan", "Clean Code", 5));
    }

    [Fact]
    public void RegisterBook_WithInvalidCirculation_ThrowsArgumentException()
    {
        var writer = new User("Anna", new[] { UserRole.Writer });
        _users.Setup(r => r.FindByName("Anna")).Returns(writer);
        var service = CreateService();

        Assert.Throws<ArgumentException>(() => service.RegisterBook("Anna", "Clean Code", 0));
    }
}
