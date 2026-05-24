namespace Library.Domain.Exceptions;

public abstract class NotFoundException : LibraryException
{
    protected NotFoundException(string message)
        : base(message)
    {
    }
}
