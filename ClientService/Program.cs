using ClientService.db;
using ClientService.filter;
using ClientService.kafka.consumer;
using ClientService.kafka.producer;
using ClientService.profile;
using ClientService.repository;
using ClientService.repository.impl;
using ClientService.service;
using ClientService.service.impl;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvcCore()
        .AddApiExplorer();

// MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddScoped<MongoDbContext>();

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Service
builder.Services.AddScoped<IUserService, UserService>();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ClientService", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

// Metrics 
builder.Services.AddMetrics();
builder.Services.AddHealthChecks();

// Mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
}, typeof(Program).Assembly);

// Exception handlers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Kafka
builder.Services.AddOptions<ProducerSettings>()
    .Bind(builder.Configuration.GetSection("KafkaProducer"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var kafkaProducerSettings = sp.GetRequiredService<IOptions<ProducerSettings>>().Value;
    var config = new ProducerConfig
    {
        BootstrapServers = kafkaProducerSettings.BootstrapServers,
        Acks = kafkaProducerSettings.Acks,
        EnableIdempotence = kafkaProducerSettings.EnableIdempotence,
        MessageSendMaxRetries = kafkaProducerSettings.MessageSendMaxRetries,
        RetryBackoffMs = kafkaProducerSettings.RetryBackoffMs,
        RequestTimeoutMs = kafkaProducerSettings.RequestTimeoutMs,
        BatchSize = kafkaProducerSettings.BatchSize,
        LingerMs = kafkaProducerSettings.LingerMs,
        CompressionType = kafkaProducerSettings.CompressionType,
        MaxInFlight = kafkaProducerSettings.MaxInFlightRequestsPerConnection,
        ConnectionsMaxIdleMs = kafkaProducerSettings.ConnectionMaxIdleMs,
        ReconnectBackoffMs = kafkaProducerSettings.ReconnectBackoffMs,
        ReconnectBackoffMaxMs = kafkaProducerSettings.ReconnectBackoffMaxMs,
        AllowAutoCreateTopics = kafkaProducerSettings.AllowAutoCreateTopics
    };

    var producerBuilder = new ProducerBuilder<string, string>(config);
    producerBuilder.SetErrorHandler((_, error) =>
    {
        var logger = sp.GetRequiredService<ILogger<IProducer<string, string>>>();
        logger.LogError("Kafka producer error: {Error}", error.Reason);
    });

    return producerBuilder.Build();
});
builder.Services.AddScoped<IKafkaMessageProducer, KafkaMessageProducer>();

builder.Services.AddOptions<ConsumerSettings>()
    .Bind(builder.Configuration.GetSection("KafkaConsumer"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Controllers
app.UseRouting();
app.MapControllers();

// Metrics
app.UseMetricServer();
app.UseHttpMetrics();

//await Task.Run(async () =>
//{
//    await Task.Delay(10000); // Даем хосту запуститься
//    using var scope = app.Services.CreateScope();
//    var consumer = scope.ServiceProvider.GetRequiredService<ConsumerService>();
//    await consumer.StartAsync(CancellationToken.None);
//});

app.Run();
