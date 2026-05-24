using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly LibraryDbContext _context;

    public UserRepository(LibraryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public User? FindByName(string name)
        => _context.Users.Find(name);

    public User Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();

        return user;
    }
}
