using Library.Domain.Enums;
using Library.Domain.Exceptions;

namespace Library.Domain.Entities;

public sealed class Request
{
    private const int NoCopies = 0;
    private const int MinCopiesForAddBook = 1;

    public Request(Guid id, string requesterName, Guid bookId, RequestType type, int copiesCount)
    {
        if (string.IsNullOrWhiteSpace(requesterName))
            throw new ArgumentException("Requester name is empty.", nameof(requesterName));

        if (!Enum.IsDefined(type))
            throw new ArgumentException("Request type is undefined.", nameof(type));

        if (type == RequestType.AddBook && copiesCount < MinCopiesForAddBook)
            throw new ArgumentException("Add-book request must add a positive number of copies.", nameof(copiesCount));

        if (type != RequestType.AddBook && copiesCount != NoCopies)
            throw new ArgumentException("Only an add-book request can specify a copies count.", nameof(copiesCount));

        Id = id;
        RequesterName = requesterName;
        BookId = bookId;
        Type = type;
        CopiesCount = copiesCount;

        Status = RequestStatus.Pending;
    }

    // Parameterless constructor used only by EF Core when materializing entities from the database.
    private Request()
    {
        RequesterName = string.Empty;
    }

    public Guid Id { get; }

    public string RequesterName { get; }

    public Guid BookId { get; }

    public RequestType Type { get; }

    public int CopiesCount { get; }

    public RequestStatus Status { get; private set; }

    public void Approve()
    {
        EnsurePending();

        Status = RequestStatus.Approved;
    }

    public void Reject()
    {
        EnsurePending();

        Status = RequestStatus.Rejected;
    }

    private void EnsurePending()
    {
        if (Status != RequestStatus.Pending)
            throw new RequestAlreadyProcessedException(Id);
    }
}
