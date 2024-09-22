namespace VerticalSliceTemplate.Api.Shared.Messaging.Messges;

public record AuthorDeleted(Guid AuthorId) : IAuthorDeleted;