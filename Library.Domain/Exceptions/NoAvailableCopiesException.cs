namespace Library.Domain.Exceptions;

public sealed class NoAvailableCopiesException : LibraryException
{
    public NoAvailableCopiesException(string bookTitle)
        : base($"Book '{bookTitle}' has no available copies to borrow.")
    {
    }
}
