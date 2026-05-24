namespace Library.Domain.Exceptions;

public sealed class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(Guid bookId)
        : base($"Book with id '{bookId}' was not found.")
    {
    }
}
