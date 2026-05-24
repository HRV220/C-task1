namespace Library.Domain.Exceptions;

public sealed class RequestAlreadyProcessedException : LibraryException
{
    public RequestAlreadyProcessedException(Guid requestId)
        : base($"Request with id '{requestId}' has already been processed.")
    {
    }
}
