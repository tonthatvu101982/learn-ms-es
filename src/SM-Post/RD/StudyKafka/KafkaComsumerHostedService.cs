using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace StudyKafka
{
    public class KafkaComsumerHostedService : IHostedService
    {
        private readonly ILogger<KafkaComsumerHostedService> _logger;
        private readonly ClusterClient _cluster;

        public KafkaComsumerHostedService(ILogger<KafkaComsumerHostedService> logger
            )
        {
            _logger = logger;
            _cluster = new ClusterClient(new Configuration
            {
                Seeds = "localhost:9092"
            }, new ConsoleLogger());
        }
        public  Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("demo");
            _cluster.MessageReceived += record =>
            {
                _logger.LogInformation($"Received: {Encoding.UTF8.GetString(record.Value as byte[])}");
            };
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
           _cluster?.Dispose();
            return Task.CompletedTask;
        }
    }
}
