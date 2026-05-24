using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;

namespace Library.Application.Services;

public sealed class RequestService : IRequestService
{
    private const int NoCopies = 0;

    private readonly IUserRepository _users;
    private readonly IBookRepository _books;
    private readonly IRequestRepository _requests;

    public RequestService(IUserRepository users, IBookRepository books, IRequestRepository requests)
    {
        ArgumentNullException.ThrowIfNull(users);
        ArgumentNullException.ThrowIfNull(books);
        ArgumentNullException.ThrowIfNull(requests);

        _users = users;
        _books = books;
        _requests = requests;
    }

    public Request CreateBorrowRequest(string readerName, Guid bookId)
    {
        if (string.IsNullOrWhiteSpace(readerName))
            throw new ArgumentException("Reader name is empty.", nameof(readerName));

        var reader = GetUser(readerName);
        EnsureRole(reader, UserRole.Reader);

        var book = GetBook(bookId);

        if (reader.HasBorrowed(book.Id))
            throw new BookAlreadyBorrowedException(reader.Name, book.Id);

        var request = new Request(Guid.NewGuid(), reader.Name, book.Id, RequestType.BorrowBook, NoCopies);

        return SubmitRequest(request, reader);
    }

    public Request CreateReturnRequest(string readerName, Guid bookId)
    {
        if (string.IsNullOrWhiteSpace(readerName))
            throw new ArgumentException("Reader name is empty.", nameof(readerName));

        var reader = GetUser(readerName);
        EnsureRole(reader, UserRole.Reader);

        var book = GetBook(bookId);

        if (!reader.HasBorrowed(book.Id))
            throw new BookNotBorrowedException(reader.Name, book.Id);

        var request = new Request(Guid.NewGuid(), reader.Name, book.Id, RequestType.ReturnBook, NoCopies);

        return SubmitRequest(request, reader);
    }

    public Request CreateAddBookRequest(string writerName, Guid bookId, int copiesCount)
    {
        if (string.IsNullOrWhiteSpace(writerName))
            throw new ArgumentException("Writer name is empty.", nameof(writerName));

        var writer = GetUser(writerName);
        EnsureRole(writer, UserRole.Writer);

        var book = GetBook(bookId);

        if (!string.Equals(book.AuthorName, writer.Name, StringComparison.Ordinal))
            throw new BookOwnershipException(writer.Name, book.Id);

        var request = new Request(Guid.NewGuid(), writer.Name, book.Id, RequestType.AddBook, copiesCount);

        if (book.CopiesInLibrary + copiesCount > book.Circulation)
            throw new CirculationExceededException(book.Title);

        return SubmitRequest(request, writer);
    }

    public Request Approve(Guid requestId, string librarianName)
    {
        EnsureLibrarian(librarianName);

        var request = GetRequest(requestId);

        request.Approve();
        ApplyApprovedEffect(request);

        return _requests.Update(request);
    }

    public Request Reject(Guid requestId, string librarianName)
    {
        EnsureLibrarian(librarianName);

        var request = GetRequest(requestId);

        request.Reject();

        return _requests.Update(request);
    }

    private Request SubmitRequest(Request request, User creator)
    {
        if (creator.HasRole(UserRole.Librarian))
        {
            request.Approve();
            ApplyApprovedEffect(request);
        }

        return _requests.Add(request);
    }

    private void ApplyApprovedEffect(Request request)
    {
        switch (request.Type)
        {
            case RequestType.BorrowBook:
                ApplyBorrow(request);
                break;
            case RequestType.ReturnBook:
                ApplyReturn(request);
                break;
            case RequestType.AddBook:
                ApplyAddBook(request);
                break;
            default:
                throw new InvalidOperationException($"Unsupported request type '{request.Type}'.");
        }
    }

    private void ApplyBorrow(Request request)
    {
        var reader = GetUser(request.RequesterName);
        var book = GetBook(request.BookId);

        book.Borrow();
        reader.BorrowBook(book.Id);

        _books.Update(book);
        _users.Update(reader);
    }

    private void ApplyReturn(Request request)
    {
        var reader = GetUser(request.RequesterName);
        var book = GetBook(request.BookId);

        reader.ReturnBook(book.Id);
        book.Return();

        _books.Update(book);
        _users.Update(reader);
    }

    private void ApplyAddBook(Request request)
    {
        var book = GetBook(request.BookId);

        book.AddCopies(request.CopiesCount);

        _books.Update(book);
    }

    private User GetUser(string name)
        => _users.FindByName(name) ?? throw new UserNotFoundException(name);

    private Book GetBook(Guid bookId)
        => _books.FindById(bookId) ?? throw new BookNotFoundException(bookId);

    private Request GetRequest(Guid requestId)
        => _requests.FindById(requestId) ?? throw new RequestNotFoundException(requestId);

    private void EnsureLibrarian(string librarianName)
    {
        if (string.IsNullOrWhiteSpace(librarianName))
            throw new ArgumentException("Librarian name is empty.", nameof(librarianName));

        var librarian = GetUser(librarianName);
        EnsureRole(librarian, UserRole.Librarian);
    }

    private void EnsureRole(User user, UserRole role)
    {
        if (!user.HasRole(role))
            throw new MissingRoleException(user.Name, role);
    }
}
