using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StudyKafka
{
    public class KafkaProducerHostedServices : IHostedService
    {
        private readonly ILogger<KafkaProducerHostedServices> _logger;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerHostedServices(ILogger<KafkaProducerHostedServices> logger)
        {
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 100; i++)
            {
                var value = $"Hello world {i}";
                await _producer.ProduceAsync("demo", new Message<Null, string> { Value = value }, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer.Dispose();
            return Task.CompletedTask;
        }
    }
}
