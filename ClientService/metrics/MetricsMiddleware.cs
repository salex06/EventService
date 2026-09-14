using Prometheus;
using System.Diagnostics;

namespace ClientService.metrics
{
    public class MetricsMiddleware
    {
        private readonly RequestDelegate _next;

        public MetricsMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/metrics"))
            {
                await _next(context);
                return;
            }

            if (context.Request.Path.StartsWithSegments("/graphql"))
            {
                await traceGraphql(context);
            }
            else
            {
                await traceRest(context);
            }
        }

        public async Task traceRest(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();

                var route = context.GetEndpoint() is RouteEndpoint re
                    ? re.RoutePattern.RawText ?? "unknown"
                    : context.Request.Path.Value ?? "unknown";

                CustomMetrics.RestRequestDuration
                    .WithLabels(
                        route,
                        context.Request.Method,
                        context.Response.StatusCode.ToString())
                    .Observe(sw.Elapsed.TotalSeconds);
            }
        }

        public async Task traceGraphql(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try { await _next(context); }
            finally
            {
                sw.Stop();
                CustomMetrics.GraphQlRequestDuration
                    .WithLabels("http", "graphql", context.Response.StatusCode.ToString())
                    .Observe(sw.Elapsed.TotalSeconds);
            }
        }
    }
}