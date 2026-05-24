using Library.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Library.Tests.Infrastructure;

public abstract class SqliteContextTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<LibraryDbContext> _options;

    protected SqliteContextTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    protected LibraryDbContext CreateContext()
        => new(_options);

    public void Dispose()
        => _connection.Dispose();
}
