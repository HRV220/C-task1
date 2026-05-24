using System.Linq.Expressions;
using Library.Api.Contracts.Responses;
using Library.Domain.Entities;

namespace Library.Api.Mapping;

public static class RequestMappingExtensions
{
    private static readonly Expression<Func<Request, RequestResponse>> Mapping =
        request => new RequestResponse
        {
            Id = request.Id,
            RequesterName = request.RequesterName,
            BookId = request.BookId,
            Type = request.Type.ToString(),
            CopiesCount = request.CopiesCount,
            Status = request.Status.ToString(),
        };

    private static readonly Func<Request, RequestResponse> CompiledMapping = Mapping.Compile();

    public static Expression<Func<Request, RequestResponse>> ToResponseExpression => Mapping;

    public static RequestResponse ToResponse(this Request request)
        => CompiledMapping(request);
}
