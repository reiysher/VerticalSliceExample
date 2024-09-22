using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VerticalSliceTemplate.Api.Shared.Persistence.Configurations;

internal sealed class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(author => author.Id);
        builder.ToTable("Authors");

        builder.Property(author => author.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder.HasMany(author => author.Notes)
            .WithOne()
            .HasForeignKey(note => note.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}