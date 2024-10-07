using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VerticalSliceTemplate.Api.Shared.Entities;

namespace VerticalSliceTemplate.Api.Shared.Persistence.Configurations;

public sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(note => note.Id);
        builder.ToTable("Notes");

        builder.Property(note => note.Id)
            .ValueGeneratedNever()
            .IsRequired();
    }
}