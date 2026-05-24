namespace Library.Domain.Exceptions;

public sealed class CirculationExceededException : LibraryException
{
    public CirculationExceededException(string bookTitle)
        : base($"Adding copies exceeds the circulation of book '{bookTitle}'.")
    {
    }
}
