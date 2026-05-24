using Library.Domain.Exceptions;

namespace Library.Domain.Entities;

public sealed class Book
{
    private const int MinCirculation = 1;
    private const int MinCopiesToAdd = 1;

    public Book(Guid id, string title, string authorName, int circulation)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Book title is empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(authorName))
            throw new ArgumentException("Book author name is empty.", nameof(authorName));

        if (circulation < MinCirculation)
            throw new ArgumentException("Book circulation must be positive.", nameof(circulation));

        Id = id;
        Title = title;
        AuthorName = authorName;
        Circulation = circulation;

        CopiesInLibrary = 0;
        AvailableCopies = 0;
    }

    // Parameterless constructor used only by EF Core when materializing entities from the database.
    private Book()
    {
        Title = string.Empty;
        AuthorName = string.Empty;
    }

    public Guid Id { get; }

    public string Title { get; }

    public string AuthorName { get; }

    public int Circulation { get; }

    public int CopiesInLibrary { get; private set; }

    public int AvailableCopies { get; private set; }

    public void AddCopies(int count)
    {
        if (count < MinCopiesToAdd)
            throw new ArgumentException("Copies count to add must be positive.", nameof(count));

        if (CopiesInLibrary + count > Circulation)
            throw new CirculationExceededException(Title);

        CopiesInLibrary += count;
        AvailableCopies += count;
    }

    public void Borrow()
    {
        if (AvailableCopies <= 0)
            throw new NoAvailableCopiesException(Title);

        AvailableCopies--;
    }

    public void Return()
    {
        if (AvailableCopies >= CopiesInLibrary)
            throw new InvalidOperationException(
                $"Cannot return a copy of book '{Title}': all copies are already in the library.");

        AvailableCopies++;
    }
}
