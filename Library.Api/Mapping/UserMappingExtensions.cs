using System.Linq.Expressions;
using Library.Api.Contracts.Responses;
using Library.Domain.Entities;

namespace Library.Api.Mapping;

public static class UserMappingExtensions
{
    private static readonly Expression<Func<User, UserResponse>> Mapping =
        user => new UserResponse
        {
            Name = user.Name,
            Roles = user.Roles.Select(role => role.ToString()).ToList(),
            BorrowedBooks = user.BorrowedBooks.ToList(),
        };

    private static readonly Func<User, UserResponse> CompiledMapping = Mapping.Compile();

    public static Expression<Func<User, UserResponse>> ToResponseExpression => Mapping;

    public static UserResponse ToResponse(this User user)
        => CompiledMapping(user);
}
