using Confluent.Kafka;
using CQRS.Core.Domains;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using MongoDB.Bson.Serialization;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastrusture.Config;
using Post.Cmd.Infrastrusture.Dispatchers;
using Post.Cmd.Infrastrusture.Handler;
using Post.Cmd.Infrastrusture.Producer;
using Post.Cmd.Infrastrusture.Repositories;
using Post.Cmd.Infrastrusture.Stores;
using Post.Common.Events;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        BsonClassMap.RegisterClassMap<BaseEvent>();
        BsonClassMap.RegisterClassMap<PostCreatedEvent>();
        BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
        BsonClassMap.RegisterClassMap<PostLikedEvent>();
        BsonClassMap.RegisterClassMap<CommentAddedEvent>();
        BsonClassMap.RegisterClassMap<CommentRemoveEvent>();
        BsonClassMap.RegisterClassMap<PostRemoveEvent>();
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services
            .Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));

        builder.Services
            .Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
        builder.Services.AddScoped<IEventStore, EventStore>();
        builder.Services.AddScoped<IEventSourceHandler<PostAggregate>, EventSourcingHandler>();
        builder.Services.AddScoped<ICommandHadler, CommandHandler>();
        builder.Services.AddScoped<IEventProducer, EventProducer>();

        var commandHandler = builder.Services.BuildServiceProvider().GetRequiredService<ICommandHadler>();

        var dispatcher = new CommandDispatcher();
        dispatcher.RegisterHandler<NewPostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditMessageCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<LikePostCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<AddCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<EditCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<RemoveCommentCommand>(commandHandler.HandleAsync);
        dispatcher.RegisterHandler<DeletePostCommand>(commandHandler.HandleAsync);

        builder.Services.AddSingleton<ICommandDispatcher>(_ => dispatcher);


        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapControllers();
      

        app.Run();
    }
}

