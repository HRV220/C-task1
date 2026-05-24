using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public Book Add(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();

        return book;
    }

    public Book? FindById(Guid id)
        => _context.Books.Find(id);

    public IReadOnlyCollection<Book> FindByAuthor(string authorName)
        => _context.Books
            .Where(book => book.AuthorName.Equals(authorName))
            .ToList();

    public Book Update(Book book)
    {
        _context.Books.Update(book);
        _context.SaveChanges();

        return book;
    }
}
