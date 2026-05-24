namespace Library.Api.Contracts.Responses;

public sealed record BookResponse
{
    public required Guid Id { get; init; }

    public required string Title { get; init; }

    public required string AuthorName { get; init; }

    public required int Circulation { get; init; }

    public required int CopiesInLibrary { get; init; }

    public required int AvailableCopies { get; init; }
}
