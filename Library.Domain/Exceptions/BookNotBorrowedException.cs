namespace Library.Domain.Exceptions;

public sealed class BookNotBorrowedException : LibraryException
{
    public BookNotBorrowedException(string userName, Guid bookId)
        : base($"Reader '{userName}' has not borrowed book '{bookId}'.")
    {
    }
}
