using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Services;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;

namespace Library.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users)
    {
        ArgumentNullException.ThrowIfNull(users);

        _users = users;
    }

    public User Register(string name, IReadOnlyCollection<UserRole> roles)
    {
        var user = new User(name, roles);

        if (_users.FindByName(user.Name) is not null)
            throw new DuplicateUserException(user.Name);

        return _users.Add(user);
    }

    public User GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is empty.", nameof(name));

        var user = _users.FindByName(name);

        return user ?? throw new UserNotFoundException(name);
    }
}
