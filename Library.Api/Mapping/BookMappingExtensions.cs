using System.Linq.Expressions;
using Library.Api.Contracts.Responses;
using Library.Domain.Entities;

namespace Library.Api.Mapping;

public static class BookMappingExtensions
{
    private static readonly Expression<Func<Book, BookResponse>> Mapping =
        book => new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            AuthorName = book.AuthorName,
            Circulation = book.Circulation,
            CopiesInLibrary = book.CopiesInLibrary,
            AvailableCopies = book.AvailableCopies,
        };

    private static readonly Func<Book, BookResponse> CompiledMapping = Mapping.Compile();

    public static Expression<Func<Book, BookResponse>> ToResponseExpression => Mapping;

    public static BookResponse ToResponse(this Book book)
        => CompiledMapping(book);
}
