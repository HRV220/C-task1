using Library.Domain.Entities;

namespace Library.Application.Abstractions.Repositories;

public interface IUserRepository
{
    User Add(User user);

    User? FindByName(string name);

    User Update(User user);
}
