using Library.Domain.Enums;

namespace Library.Api.Contracts.Requests;

public sealed record RegisterUserRequest(string Name, IReadOnlyCollection<UserRole> Roles);
