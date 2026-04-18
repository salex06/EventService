using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MS_Lab.config;
using MS_Lab.data;
using MS_Lab.filter;
using MS_Lab.kafka.consumer;
using MS_Lab.kafka.producer;
using MS_Lab.profiles;
using MS_Lab.repositories.events;
using MS_Lab.repositories.tickets;
using MS_Lab.services.events;
using MS_Lab.services.tickets;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvcCore()
        .AddApiExplorer();

// Mongo
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

// Custom config
builder.Services.AddOptions<RepositoryConfig>()
    .Bind(builder.Configuration.GetSection(RepositoryConfig.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Mapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<EventProfile>();
    cfg.AddProfile<TicketProfile>();
}, typeof(Program).Assembly);

// Exception handlers
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiExceptionFilterAttribute>();
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis")["ConnectionString"];
    options.InstanceName = "MSLab:"; // префикс для ключей
});

// Metrics and visualization
builder.Services.AddMetrics();
builder.Services.AddHealthChecks();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MS_Lab", Version = "v1" });
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

// Injected objects
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

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

//Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MS Lab API V1");
});

// Exception handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

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

await app.RunAsync();
public partial class Program 
{
    protected Program() { }
}