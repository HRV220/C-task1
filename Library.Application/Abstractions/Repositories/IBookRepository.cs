using Library.Domain.Entities;

namespace Library.Application.Abstractions.Repositories;

public interface IBookRepository
{
    Book Add(Book book);

    Book? FindById(Guid id);

    IReadOnlyCollection<Book> FindByAuthor(string authorName);

    Book Update(Book book);
}
