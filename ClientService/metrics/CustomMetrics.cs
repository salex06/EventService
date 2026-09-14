using Prometheus;

namespace ClientService.metrics
{
    public static class CustomMetrics
    {
        // Гистограмма для REST
        public static readonly Histogram RestRequestDuration = Metrics.CreateHistogram(
            "app_rest_request_duration_seconds",
            "Длительность REST-запроса",
            new HistogramConfiguration
            {
                LabelNames = new[] { "route", "method", "status" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 15) // 1ms .. ~32s
            });

        // Гистограмма для GraphQL
        public static readonly Histogram GraphQlRequestDuration = Metrics.CreateHistogram(
            "app_graphql_request_duration_seconds",
            "Длительность GraphQL-запроса",
            new HistogramConfiguration
            {
                LabelNames = new[] { "operation_type", "operation_name", "status" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 15)
            });
    }
}