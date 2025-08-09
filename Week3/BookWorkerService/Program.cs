using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Week3.BookWorkerService.Consumers;

namespace Week3.BookWorkerService;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddMassTransit(x =>
                {
                    x.AddConsumer<BookCreatedEventConsumer>();

                    x.UsingRabbitMq((ctx, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        // Tự động bind consumer vào queue
                        cfg.ReceiveEndpoint("book-created-queue", e =>
                        {
                            e.ConfigureConsumer<BookCreatedEventConsumer>(ctx);
                        });
                    });
                });

                services.AddMassTransitHostedService(true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            });
}
