namespace Library.Domain.Exceptions;

public class LibraryException : Exception
{
    public LibraryException(string message)
        : base(message)
    {
    }
}
