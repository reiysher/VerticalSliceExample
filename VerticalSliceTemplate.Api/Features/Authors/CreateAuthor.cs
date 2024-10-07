using FluentValidation;
using VerticalSliceTemplate.Api.Shared.Entities;

namespace VerticalSliceTemplate.Api.Features.Authors;

// Slice
public static class CreateAuthor
{
    // Endpoint
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {

            app.MapPost("authors", Create)
                .Accepts<Request>(MediaTypeNames.Application.Json)
                .AllowAnonymous();
        }

        private static async Task<IResult> Create(
            [FromBody] Request request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken)
        {
            return await sender.Send(request, cancellationToken);
        }
    }

    // Request
    public sealed record Request(string FirstName, DateTimeOffset BirthDay) : IRequest<IResult>;

    // Validator
    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty();

            RuleFor(x => x.BirthDay)
                .NotEmpty();
        }
    }

    // Handler
    public sealed class Handler(ApplicationDbContext dbContext, TimeProvider timeProvider)
        : IRequestHandler<Request, IResult>
    {
        public async Task<IResult> Handle(Request request, CancellationToken cancellationToken)
        {
            var author = new Author
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = null,
                MiddleName = null,
                BirthDay = request.BirthDay,
                Created = timeProvider.GetUtcNow(),
                IsDeleted = false
            };

            dbContext.Set<Author>().Add(author);
            await dbContext.SaveChangesAsync(cancellationToken);

            var authorDto = new AuthorDto(author.Id, author.FirstName, author.BirthDay);

            return Results.Created($"authors/{author.Id}", authorDto);
        }
    }

    // Response
    public sealed record AuthorDto(Guid Id, string FirstName, DateTimeOffset BirthDay);
}