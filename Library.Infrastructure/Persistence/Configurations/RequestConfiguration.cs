using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.HasKey(request => request.Id);

        builder.Property(request => request.BookId);

        builder.Property(request => request.RequesterName)
            .HasMaxLength(200);

        builder.Property(request => request.CopiesCount);

        builder.Property(request => request.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(request => request.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
    }
}
