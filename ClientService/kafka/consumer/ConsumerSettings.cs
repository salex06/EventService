namespace ClientService.kafka.consumer
{
    public record ConsumerSettings
    {
        public string BootstrapServers { get; set; } = "";
        public string GroupId { get; set; } = "";
        public string Topic { get; set; } = "";
    }
}
