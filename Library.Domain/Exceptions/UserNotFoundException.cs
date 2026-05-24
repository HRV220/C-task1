namespace Library.Domain.Exceptions;

public sealed class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string userName)
        : base($"User with name '{userName}' was not found.")
    {
    }
}
