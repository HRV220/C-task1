namespace Library.Domain.Exceptions;

public sealed class DuplicateUserException : LibraryException
{
    public DuplicateUserException(string userName)
        : base($"User with name '{userName}' already exists.")
    {
    }
}
