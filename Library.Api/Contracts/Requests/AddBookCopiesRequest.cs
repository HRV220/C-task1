namespace Library.Api.Contracts.Requests;

public sealed record AddBookCopiesRequest(string WriterName, int CopiesCount);
