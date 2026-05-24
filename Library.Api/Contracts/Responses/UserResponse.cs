namespace Library.Api.Contracts.Responses;

public sealed record UserResponse
{
    public required string Name { get; init; }

    public required IReadOnlyCollection<string> Roles { get; init; }

    public required IReadOnlyCollection<Guid> BorrowedBooks { get; init; }
}
