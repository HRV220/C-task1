namespace Library.Api.Contracts.Responses;

public sealed record RequestResponse
{
    public required Guid Id { get; init; }

    public required string RequesterName { get; init; }

    public required Guid BookId { get; init; }

    public required string Type { get; init; }

    public required int CopiesCount { get; init; }

    public required string Status { get; init; }
}
