using MassTransit;
using VerticalSliceTemplate.Api.Shared.Entities;
using VerticalSliceTemplate.Api.Shared.Messaging.Messges;

namespace VerticalSliceTemplate.Api.Features.Authors;

public sealed class DeleteAuthor
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("authors", Delete)
                .Accepts<Request>(MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        private static async Task<IResult> Delete(
            [FromBody] Request request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            return await sender.Send(request, cancellationToken);
        }
    }

    public sealed record Request(Guid AuthorId) : IRequest<IResult>;

    public sealed class Handler(ApplicationDbContext dbContext, IBus bus) : IRequestHandler<Request, IResult>
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

            author.IsDeleted = true;
            await dbContext.SaveChangesAsync(cancellationToken);

            await bus.Publish(new AuthorDeleted(author.Id), cancellationToken);

            return Results.NoContent();
        }
    }
}