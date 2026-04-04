using ClientService.dto;
using ClientService.service;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading;
using static Prometheus.MetricServerMiddleware;

namespace ClientService.kafka.consumer
{
    public class ConsumerService : BackgroundService
    {
        private readonly ConsumerSettings _consumerSettings;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _topic;

        public ConsumerService(
            IServiceScopeFactory scopeFactory,
            IOptions<ConsumerSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _topic = settings.Value.Topic;
            _consumerSettings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(3000, stoppingToken);

            IConsumer<string, string> consumer = null;
            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _consumerSettings.BootstrapServers,
                    GroupId = _consumerSettings.GroupId,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    SessionTimeoutMs = 30000
                };

                consumer = new ConsumerBuilder<string, string>(config).Build();
                consumer.Subscribe(_consumerSettings.Topic);


                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(500));

                        if (consumeResult != null && !consumeResult.IsPartitionEOF)
                        {
                            await ProcessMessageAsync(consumeResult.Message.Value);
                        }
                    }
                    catch (ConsumeException)
                    {
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                consumer?.Unsubscribe();
                consumer?.Close();
                consumer?.Dispose();
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            try
            {
                var data = JsonSerializer.Deserialize<RegObjectDto>(message);
                if (data != null)
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await userService.ConfirmObject(data);
                    }
            }
            catch (Exception)
            {
               //It'll be better to log it
            }
        }

        //public override async Task StopAsync(CancellationToken cancellationToken)
        //{
        //    _consumer.Unsubscribe();

        //    _consumer.Close();

        //    await Task.Delay(5000, cancellationToken);

        //    _consumer.Dispose();

        //    await base.StopAsync(cancellationToken);
        //}
    }
}
