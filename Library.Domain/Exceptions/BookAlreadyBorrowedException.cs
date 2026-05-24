namespace Library.Domain.Exceptions;

public sealed class BookAlreadyBorrowedException : LibraryException
{
    public BookAlreadyBorrowedException(string userName, Guid bookId)
        : base($"Reader '{userName}' has already borrowed book '{bookId}'.")
    {
    }
}
