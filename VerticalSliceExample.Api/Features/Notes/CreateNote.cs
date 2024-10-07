using VerticalSliceExample.Api.Shared.Entities;

namespace VerticalSliceExample.Api.Features.Notes;

public static class CreateNote
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("authors/{authorId:guid}/notes", Create)
                .Accepts<Request>(MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        private Task Create(
            [FromRoute] Guid authorId,
            [FromBody] Request request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            request = request with { AuthorId = authorId }; // костыль, который сделал автор курса
            return sender.Send(request, cancellationToken);
        }
    }

    public sealed record Request(Guid AuthorId, string? Value) : IRequest<IResult>;

    public sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Request, IResult>
    {
        public async Task<IResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var note = new Note
            {
                Id = Guid.NewGuid(),
                AuthorId = request.AuthorId,
                IsDeleted = false,
                Value = request.Value
            };

            dbContext.Set<Note>().Add(note);
            await dbContext.SaveChangesAsync(cancellationToken);

            var noteDto = new NoteDto(note.Id, note.AuthorId, note.Value);

            return Results.Created($"authors/{note.AuthorId}/notes/{note.Id}", noteDto);
        }
    }

    public sealed record NoteDto(Guid Id, Guid AuthorId, string? Value);
}