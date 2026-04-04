using Confluent.Kafka;

namespace ClientService.kafka.producer
{
    public record ProducerSettings
    {
        public string BootstrapServers { get; set; } = string.Empty;
        public ISerializer<string> KeySerializer { get; set; } = Serializers.Utf8;
        public ISerializer<string> ValueSerializer { get; set; } = Serializers.Utf8;
        public Acks Acks { get; set; } = Acks.All;
        public bool EnableIdempotence { get; set; } = true;
        public int MessageSendMaxRetries { get; set; } = 10;
        public int RetryBackoffMs { get; set; } = 100;
        public int RequestTimeoutMs { get; set; } = 3000;
        public int BatchSize { get; set; } = 16384;
        public int LingerMs { get; set; } = 20;
        public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
        public int MaxInFlightRequestsPerConnection { get; set; } = 5;
        public int ConnectionMaxIdleMs { get; set; } = 54000;
        public int ReconnectBackoffMs { get; set; } = 50;
        public int ReconnectBackoffMaxMs { get; set; } = 1000;
        public bool AllowAutoCreateTopics { get; set; } = true;

        public string TopicName { get; set; } = string.Empty;
    }
}
