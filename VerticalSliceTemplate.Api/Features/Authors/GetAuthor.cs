using VerticalSliceTemplate.Api.Shared.Entities;

namespace VerticalSliceTemplate.Api.Features.Authors;

public static class GetAuthor
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("authors/{authorId:guid}", Get)
                .Produces<Response?>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        public static async Task<IResult> Get(
            [FromRoute] Guid authorId,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            var query = new Request(authorId);
            return await sender.Send(query, cancellationToken);
        }
    }

    public sealed record Request(Guid AuthorId) : IRequest<IResult>;

    public sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Request, IResult>
    {
        public async Task<IResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var author = await dbContext
                .Set<Author>()
                .Where(a => !a.IsDeleted)
                .SingleOrDefaultAsync(a => a.Id == request.AuthorId, cancellationToken);

            if (author == null)
            {
                return Results.NotFound();
            }

            string?[] items =
            [
                author.FirstName, author.LastName, author.MiddleName
            ];

            string fullName = string.Join(", ", items.Where(item => !String.IsNullOrWhiteSpace(item)));

            var response = new Response(author.Id, fullName, author.BirthDay);
            return Results.Ok(response);
        }
    }

    public sealed record Response(Guid Id, string FullName, DateTimeOffset BirthDay);
}