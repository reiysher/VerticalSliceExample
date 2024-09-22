namespace VerticalSliceTemplate.Api.Features.Notes;

public static class GetNote
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("authors/{authorId:guid}/notes/{noteId:guid}", Get)
                .Produces<Response?>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        public static async Task<IResult> Get(
            [FromRoute] Guid authorId,
            [FromRoute] Guid noteId,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var query = new Request(authorId, noteId);
            return await sender.Send(query, cancellationToken);
        }
    }

    public sealed record Request(Guid AuthorId, Guid NoteId) : IRequest<IResult>;
    
    public sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Request, IResult>
    {
        public async Task<IResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var note = await dbContext
                .Set<Note>()
                .SingleOrDefaultAsync(n => n.AuthorId == request.AuthorId && n.Id == request.NoteId, cancellationToken);

            if (note == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new Response(note.Id, note.Value));
        }
    }

    public sealed record Response(Guid Id, string? Value);
}