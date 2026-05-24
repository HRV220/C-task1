using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;

namespace Library.Application.Services;

public sealed class BookService : IBookService
{
    private readonly IUserRepository _users;
    private readonly IBookRepository _books;

    public BookService(IUserRepository users, IBookRepository books)
    {
        ArgumentNullException.ThrowIfNull(users);
        ArgumentNullException.ThrowIfNull(books);

        _users = users;
        _books = books;
    }

    public Book RegisterBook(string writerName, string title, int circulation)
    {
        if (string.IsNullOrWhiteSpace(writerName))
            throw new ArgumentException("Writer name is empty.", nameof(writerName));

        var writer = _users.FindByName(writerName) ?? throw new UserNotFoundException(writerName);

        if (!writer.HasRole(UserRole.Writer))
            throw new MissingRoleException(writer.Name, UserRole.Writer);

        var book = new Book(Guid.NewGuid(), title, writer.Name, circulation);

        return _books.Add(book);
    }
}
