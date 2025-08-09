using MassTransit;
using Microsoft.Extensions.Logging;
using Week3.Controllers;

namespace Week3.BookWorkerService.Consumers;

public class BookCreatedEventConsumer(ILogger<BookCreatedEventConsumer> logger) : IConsumer<CreateBookRequest>
{
    private readonly ILogger<BookCreatedEventConsumer> _logger = logger;

    public Task Consume(ConsumeContext<CreateBookRequest> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received Book: Title: {Title} - AuthorId: {Author}", message.Title, message.Author);
        return Task.CompletedTask;
    }
}
