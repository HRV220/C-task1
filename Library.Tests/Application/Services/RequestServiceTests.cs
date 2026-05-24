using Library.Application.Abstractions.Repositories;
using Library.Application.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;
using Moq;
using Xunit;

namespace Library.Tests.Application.Services;

public sealed class RequestServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IBookRepository> _books = new();
    private readonly Mock<IRequestRepository> _requests = new();

    public RequestServiceTests()
    {
        _requests.Setup(r => r.Add(It.IsAny<Request>())).Returns<Request>(request => request);
        _requests.Setup(r => r.Update(It.IsAny<Request>())).Returns<Request>(request => request);
        _users.Setup(r => r.Update(It.IsAny<User>())).Returns<User>(user => user);
        _books.Setup(r => r.Update(It.IsAny<Book>())).Returns<Book>(book => book);
    }

    private RequestService CreateService()
        => new(_users.Object, _books.Object, _requests.Object);

    private void SetupUser(User user)
        => _users.Setup(r => r.FindByName(user.Name)).Returns(user);

    private void SetupBook(Book book)
        => _books.Setup(r => r.FindById(book.Id)).Returns(book);

    private void SetupRequest(Request request)
        => _requests.Setup(r => r.FindById(request.Id)).Returns(request);

    private static Book CreateBook(string authorName, int circulation, int copies)
    {
        var book = new Book(Guid.NewGuid(), "Clean Code", authorName, circulation);

        if (copies > 0)
            book.AddCopies(copies);

        return book;
    }

    [Fact]
    public void CreateBorrowRequest_ByReader_CreatesPendingRequest()
    {
        var reader = new User("Ivan", new[] { UserRole.Reader });
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        SetupUser(reader);
        SetupBook(book);
        var service = CreateService();

        var request = service.CreateBorrowRequest("Ivan", book.Id);

        Assert.Equal(RequestType.BorrowBook, request.Type);
        Assert.Equal(RequestStatus.Pending, request.Status);
        _requests.Verify(r => r.Add(It.IsAny<Request>()), Times.Once);
        _books.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
        _users.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void CreateBorrowRequest_ByReaderWhoIsLibrarian_AutoApprovesAndBorrows()
    {
        var reader = new User("Ivan", new[] { UserRole.Reader, UserRole.Librarian });
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        SetupUser(reader);
        SetupBook(book);
        var service = CreateService();

        var request = service.CreateBorrowRequest("Ivan", book.Id);

        Assert.Equal(RequestStatus.Approved, request.Status);
        Assert.True(reader.HasBorrowed(book.Id));
        Assert.Equal(1, book.AvailableCopies);
        _books.Verify(r => r.Update(book), Times.Once);
        _users.Verify(r => r.Update(reader), Times.Once);
    }

    [Fact]
    public void CreateBorrowRequest_WhenReaderMissing_ThrowsUserNotFoundException()
    {
        _users.Setup(r => r.FindByName("Ghost")).Returns((User?)null);
        var service = CreateService();

        Assert.Throws<UserNotFoundException>(() => service.CreateBorrowRequest("Ghost", Guid.NewGuid()));
    }

    [Fact]
    public void CreateBorrowRequest_WhenUserIsNotReader_ThrowsMissingRoleException()
    {
        var writer = new User("Anna", new[] { UserRole.Writer });
        SetupUser(writer);
        var service = CreateService();

        Assert.Throws<MissingRoleException>(() => service.CreateBorrowRequest("Anna", Guid.NewGuid()));
    }

    [Fact]
    public void CreateBorrowRequest_WhenBookMissing_ThrowsBookNotFoundException()
    {
        var reader = new User("Ivan", new[] { UserRole.Reader });
        SetupUser(reader);
        var bookId = Guid.NewGuid();
        _books.Setup(r => r.FindById(bookId)).Returns((Book?)null);
        var service = CreateService();

        Assert.Throws<BookNotFoundException>(() => service.CreateBorrowRequest("Ivan", bookId));
    }

    [Fact]
    public void CreateBorrowRequest_WhenAlreadyBorrowed_ThrowsBookAlreadyBorrowedException()
    {
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        var reader = new User("Ivan", new[] { UserRole.Reader });
        reader.BorrowBook(book.Id);
        SetupUser(reader);
        SetupBook(book);
        var service = CreateService();

        Assert.Throws<BookAlreadyBorrowedException>(() => service.CreateBorrowRequest("Ivan", book.Id));
    }

    [Fact]
    public void CreateReturnRequest_ByReaderHoldingBook_CreatesPendingRequest()
    {
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        var reader = new User("Ivan", new[] { UserRole.Reader });
        reader.BorrowBook(book.Id);
        SetupUser(reader);
        SetupBook(book);
        var service = CreateService();

        var request = service.CreateReturnRequest("Ivan", book.Id);

        Assert.Equal(RequestType.ReturnBook, request.Type);
        Assert.Equal(RequestStatus.Pending, request.Status);
    }

    [Fact]
    public void CreateReturnRequest_ByReaderWhoIsLibrarian_AutoApprovesAndReturns()
    {
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        book.Borrow();
        var reader = new User("Ivan", new[] { UserRole.Reader, UserRole.Librarian });
        reader.BorrowBook(book.Id);
        SetupUser(reader);
        SetupBook(book);
        var service = CreateService();

        var request = service.CreateReturnRequest("Ivan", book.Id);

        Assert.Equal(RequestStatus.Approved, request.Status);
        Assert.False(reader.HasBorrowed(book.Id));
        Assert.Equal(2, book.AvailableCopies);
    }

    [Fact]
    public void CreateReturnRequest_WhenBookNotBorrowed_ThrowsBookNotBorrowedException()
    {
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        var reader = new User("Ivan", new[] { UserRole.Reader });
        SetupUser(reader);
        SetupBook(book);
        var service = CreateService();

        Assert.Throws<BookNotBorrowedException>(() => service.CreateReturnRequest("Ivan", book.Id));
    }

    [Fact]
    public void CreateAddBookRequest_ByOwningWriter_CreatesPendingRequestWithoutAddingCopies()
    {
        var writer = new User("Anna", new[] { UserRole.Writer });
        var book = CreateBook("Anna", circulation: 5, copies: 0);
        SetupUser(writer);
        SetupBook(book);
        var service = CreateService();

        var request = service.CreateAddBookRequest("Anna", book.Id, 3);

        Assert.Equal(RequestType.AddBook, request.Type);
        Assert.Equal(RequestStatus.Pending, request.Status);
        Assert.Equal(3, request.CopiesCount);
        Assert.Equal(0, book.CopiesInLibrary);
        _books.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public void CreateAddBookRequest_ByWriterWhoIsLibrarian_AutoApprovesAndAddsCopies()
    {
        var writer = new User("Anna", new[] { UserRole.Writer, UserRole.Librarian });
        var book = CreateBook("Anna", circulation: 5, copies: 0);
        SetupUser(writer);
        SetupBook(book);
        var service = CreateService();

        var request = service.CreateAddBookRequest("Anna", book.Id, 3);

        Assert.Equal(RequestStatus.Approved, request.Status);
        Assert.Equal(3, book.CopiesInLibrary);
        Assert.Equal(3, book.AvailableCopies);
        _books.Verify(r => r.Update(book), Times.Once);
    }

    [Fact]
    public void CreateAddBookRequest_WhenWriterDoesNotOwnBook_ThrowsBookOwnershipException()
    {
        var writer = new User("Anna", new[] { UserRole.Writer });
        var book = CreateBook("Robert", circulation: 5, copies: 0);
        SetupUser(writer);
        SetupBook(book);
        var service = CreateService();

        Assert.Throws<BookOwnershipException>(() => service.CreateAddBookRequest("Anna", book.Id, 3));
    }

    [Fact]
    public void CreateAddBookRequest_WhenCopiesExceedCirculation_ThrowsCirculationExceededException()
    {
        var writer = new User("Anna", new[] { UserRole.Writer });
        var book = CreateBook("Anna", circulation: 5, copies: 4);
        SetupUser(writer);
        SetupBook(book);
        var service = CreateService();

        Assert.Throws<CirculationExceededException>(() => service.CreateAddBookRequest("Anna", book.Id, 2));
    }

    [Fact]
    public void CreateAddBookRequest_WhenUserIsNotWriter_ThrowsMissingRoleException()
    {
        var reader = new User("Ivan", new[] { UserRole.Reader });
        SetupUser(reader);
        var service = CreateService();

        Assert.Throws<MissingRoleException>(() => service.CreateAddBookRequest("Ivan", Guid.NewGuid(), 3));
    }

    [Fact]
    public void Approve_BorrowRequestByLibrarian_ApprovesAndAppliesEffect()
    {
        var librarian = new User("Boss", new[] { UserRole.Librarian });
        var reader = new User("Ivan", new[] { UserRole.Reader });
        var book = CreateBook("Anna", circulation: 5, copies: 2);
        var request = new Request(Guid.NewGuid(), reader.Name, book.Id, RequestType.BorrowBook, 0);
        SetupUser(librarian);
        SetupUser(reader);
        SetupBook(book);
        SetupRequest(request);
        var service = CreateService();

        var result = service.Approve(request.Id, "Boss");

        Assert.Equal(RequestStatus.Approved, result.Status);
        Assert.True(reader.HasBorrowed(book.Id));
        Assert.Equal(1, book.AvailableCopies);
        _requests.Verify(r => r.Update(request), Times.Once);
    }

    [Fact]
    public void Approve_WhenRequestMissing_ThrowsRequestNotFoundException()
    {
        var librarian = new User("Boss", new[] { UserRole.Librarian });
        SetupUser(librarian);
        var requestId = Guid.NewGuid();
        _requests.Setup(r => r.FindById(requestId)).Returns((Request?)null);
        var service = CreateService();

        Assert.Throws<RequestNotFoundException>(() => service.Approve(requestId, "Boss"));
    }

    [Fact]
    public void Approve_ByNonLibrarian_ThrowsMissingRoleException()
    {
        var reader = new User("Ivan", new[] { UserRole.Reader });
        SetupUser(reader);
        var service = CreateService();

        Assert.Throws<MissingRoleException>(() => service.Approve(Guid.NewGuid(), "Ivan"));
    }

    [Fact]
    public void Approve_WhenRequestAlreadyProcessed_ThrowsRequestAlreadyProcessedException()
    {
        var librarian = new User("Boss", new[] { UserRole.Librarian });
        var request = new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        request.Reject();
        SetupUser(librarian);
        SetupRequest(request);
        var service = CreateService();

        Assert.Throws<RequestAlreadyProcessedException>(() => service.Approve(request.Id, "Boss"));
    }

    [Fact]
    public void Reject_PendingRequestByLibrarian_RejectsWithoutEffect()
    {
        var librarian = new User("Boss", new[] { UserRole.Librarian });
        var request = new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        SetupUser(librarian);
        SetupRequest(request);
        var service = CreateService();

        var result = service.Reject(request.Id, "Boss");

        Assert.Equal(RequestStatus.Rejected, result.Status);
        _books.Verify(r => r.Update(It.IsAny<Book>()), Times.Never);
        _users.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public void Reject_WhenRequestAlreadyProcessed_ThrowsRequestAlreadyProcessedException()
    {
        var librarian = new User("Boss", new[] { UserRole.Librarian });
        var request = new Request(Guid.NewGuid(), "Ivan", Guid.NewGuid(), RequestType.BorrowBook, 0);
        request.Approve();
        SetupUser(librarian);
        SetupRequest(request);
        var service = CreateService();

        Assert.Throws<RequestAlreadyProcessedException>(() => service.Reject(request.Id, "Boss"));
    }
}
