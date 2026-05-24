using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(user => user.Name);

        builder.Property(user => user.Name)
            .HasMaxLength(200);

        builder.PrimitiveCollection(user => user.Roles)
            .HasField("_roles")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.PrimitiveCollection(user => user.BorrowedBooks)
            .HasField("_borrowedBooks")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
