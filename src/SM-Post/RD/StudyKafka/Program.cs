// See https://aka.ms/new-console-template for more information
using Kafka.Public;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudyKafka;

 static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, collections) => {
        collections.AddHostedService<KafkaProducerHostedServices>();
        collections.AddHostedService<KafkaComsumerHostedService>();
    });

CreateHostBuilder(args).Build().Run();
