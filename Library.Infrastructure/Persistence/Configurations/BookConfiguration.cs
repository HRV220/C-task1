using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(book => book.Id);

        builder.Property(book => book.Title)
            .HasMaxLength(500);

        builder.Property(book => book.AuthorName)
            .HasMaxLength(200);

        builder.Property(book => book.Circulation);

        builder.Property(book => book.CopiesInLibrary);

        builder.Property(book => book.AvailableCopies);
    }
}
