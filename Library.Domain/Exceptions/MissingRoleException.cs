using Library.Domain.Enums;

namespace Library.Domain.Exceptions;

public sealed class MissingRoleException : LibraryException
{
    public MissingRoleException(string userName, UserRole role)
        : base($"User '{userName}' does not have the required role '{role}'.")
    {
    }
}
