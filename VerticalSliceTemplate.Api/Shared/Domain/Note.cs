namespace VerticalSliceTemplate.Api.Shared.Domain;

public sealed class Note
{
    public required Guid Id { get; init; }
    
    public required Guid AuthorId { get; init; }
    
    public required string? Value { get; set; }
    
    public required bool IsDeleted { get; set; }
}