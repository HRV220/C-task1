using Library.Domain.Enums;
using Library.Domain.Exceptions;

namespace Library.Domain.Entities;

public sealed class User
{
    private List<UserRole> _roles;
    private List<Guid> _borrowedBooks;

    public User(string name, IReadOnlyCollection<UserRole> roles)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("User name is empty.", nameof(name));

        if (roles is null || roles.Count == 0)
            throw new ArgumentException("User must have at least one role.", nameof(roles));

        _borrowedBooks = new List<Guid>();
        _roles = roles.Distinct().ToList();

        Name = name;
    }

    // Parameterless constructor used only by EF Core when materializing entities from the database.
    private User()
    {
        _roles = new List<UserRole>();
        _borrowedBooks = new List<Guid>();
        Name = string.Empty;
    }

    public string Name { get; }

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public IReadOnlyCollection<Guid> BorrowedBooks => _borrowedBooks;

    public bool HasRole(UserRole role)
        => _roles.Contains(role);

    public bool HasBorrowed(Guid bookId)
        => _borrowedBooks.Contains(bookId);

    public void BorrowBook(Guid bookId)
    {
        if (_borrowedBooks.Contains(bookId))
            throw new BookAlreadyBorrowedException(Name, bookId);

        _borrowedBooks.Add(bookId);
    }

    public void ReturnBook(Guid bookId)
    {
        if (!_borrowedBooks.Remove(bookId))
            throw new BookNotBorrowedException(Name, bookId);
    }
}
