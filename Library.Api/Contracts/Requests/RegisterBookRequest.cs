namespace Library.Api.Contracts.Requests;

public sealed record RegisterBookRequest(string WriterName, string Title, int Circulation);
