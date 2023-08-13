using HashProcessor.Worker;
using HashProcessor.Worker.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddRedis(hostContext.Configuration);
                services.AddRabbitMQ(hostContext.Configuration);
                services.AddPersistence(hostContext.Configuration);

                services.AddHostedService<RabbitMQConsumerJob>();
            });
}
