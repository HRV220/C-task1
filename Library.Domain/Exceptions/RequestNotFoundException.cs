namespace Library.Domain.Exceptions;

public sealed class RequestNotFoundException : NotFoundException
{
    public RequestNotFoundException(Guid requestId)
        : base($"Request with id '{requestId}' was not found.")
    {
    }
}
