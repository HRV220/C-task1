using Library.Domain.Entities;

namespace Library.Application.Abstractions.Repositories;

public interface IRequestRepository
{
    Request Add(Request request);

    Request? FindById(Guid id);

    IReadOnlyCollection<Request> GetPending();

    Request Update(Request request);
}
