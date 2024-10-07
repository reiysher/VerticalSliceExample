namespace VerticalSliceExample.Api.Shared.Entities;

public sealed class Author
{
    public required Guid Id { get; init; }

    public required string FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MiddleName { get; set; }

    public required DateTimeOffset BirthDay { get; set; }

    public required DateTimeOffset Created { get; set; }

    public required bool IsDeleted { get; set; }

    public ICollection<Note> Notes { get; set; } = [];
}