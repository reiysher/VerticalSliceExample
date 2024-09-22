using MassTransit;
using VerticalSliceTemplate.Api.Shared.Messaging.Messges;

namespace VerticalSliceTemplate.Api.Features.Notes;

// Slice without endpoint
public static class RemoveDeletedAuthorNotes
{
    public sealed class Consumer(ISender sender) : IConsumer<IAuthorDeleted>
    {
        public Task Consume(ConsumeContext<IAuthorDeleted> context)
        {
            var request = new Request(context.Message.AuthorId);
            return sender.Send(request, context.CancellationToken);
        }
    }
    
    public sealed class ConsumerDefinition : ConsumerDefinition<Consumer>
    {
        public ConsumerDefinition()
        {
            Endpoint(options => options.Name = "app-name.notes.delete");
        }
    }

    public sealed record Request(Guid AuthorId) : IRequest;
    
    public sealed class Handler(ApplicationDbContext dbContext) : IRequestHandler<Request>
    {
        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            var notes = await dbContext
                .Set<Note>()
                .Where(n => !n.IsDeleted)
                .Where(n => n.AuthorId == request.AuthorId)
                .ToArrayAsync(cancellationToken);

            foreach (var note in notes)
            {
                note.IsDeleted = true;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}