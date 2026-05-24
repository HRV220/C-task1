using Library.Domain.Entities;
using Library.Domain.Enums;

namespace Library.Application.Abstractions.Services;

public interface IUserService
{
    User Register(string name, IReadOnlyCollection<UserRole> roles);

    User GetByName(string name);
}
