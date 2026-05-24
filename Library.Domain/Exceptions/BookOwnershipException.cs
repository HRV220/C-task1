namespace Library.Domain.Exceptions;

public sealed class BookOwnershipException : LibraryException
{
    public BookOwnershipException(string writerName, Guid bookId)
        : base($"Writer '{writerName}' does not own book '{bookId}'.")
    {
    }
}
