using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Infrastructure.Persistence;

namespace Library.Infrastructure.Repositories;

public sealed class RequestRepository : IRequestRepository
{
    private readonly LibraryDbContext _context;

    public RequestRepository(LibraryDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    public Request Add(Request request)
    {
        _context.Requests.Add(request);
        _context.SaveChanges();

        return request;
    }

    public Request? FindById(Guid id)
        => _context.Requests.Find(id);

    public IReadOnlyCollection<Request> GetPending()
        => _context.Requests
            .Where(request => request.Status == RequestStatus.Pending)
            .ToList();

    public Request Update(Request request)
    {
        _context.Requests.Update(request);
        _context.SaveChanges();

        return request;
    }
}
