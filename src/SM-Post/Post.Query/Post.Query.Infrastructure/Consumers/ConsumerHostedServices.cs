using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Infrastructure.Consumers;

public class ConsumerHostedServices : IHostedService
{
    private readonly ILogger<ConsumerHostedServices> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConsumerHostedServices(ILogger<ConsumerHostedServices> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event consumer service  runnning");
        using var scope = _serviceProvider.CreateScope();
        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
        Task.Run(() => eventConsumer.Consume(topic), cancellationToken );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Event consumer service stopped");
        return Task.CompletedTask;
    }
}
