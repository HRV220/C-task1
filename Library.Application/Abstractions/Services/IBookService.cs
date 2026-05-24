using Library.Domain.Entities;

namespace Library.Application.Abstractions.Services;

public interface IBookService
{
    Book RegisterBook(string writerName, string title, int circulation);
}
