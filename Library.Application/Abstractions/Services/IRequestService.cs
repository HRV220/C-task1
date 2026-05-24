using Library.Domain.Entities;

namespace Library.Application.Abstractions.Services;

public interface IRequestService
{
    Request CreateBorrowRequest(string readerName, Guid bookId);

    Request CreateReturnRequest(string readerName, Guid bookId);

    Request CreateAddBookRequest(string writerName, Guid bookId, int copiesCount);

    Request Approve(Guid requestId, string librarianName);

    Request Reject(Guid requestId, string librarianName);
}
