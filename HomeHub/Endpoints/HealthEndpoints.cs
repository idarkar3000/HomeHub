using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeHub.Endpoints
{
    public static class HealthEndpoints
    {
        // GET y HEAD porque UptimeRobot hace HEAD y Render hace GET para mantener el servicio despierto
        public static void MapHealthEndpoints(this WebApplication app)
        {
            app.MapMethods("/", ["GET", "HEAD"], () => Results.Ok(new { status = "healthy", app = "HomeHub" }));

            app.MapMethods("/health", ["GET", "HEAD"], async (
                HealthCheckService healthCheckService,
                CancellationToken cancellationToken) =>
            {
                var report = await healthCheckService.CheckHealthAsync(cancellationToken);

                var payload = new
                {
                    status = report.Status.ToString().ToLowerInvariant(),
                    time = DateTime.UtcNow,
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString().ToLowerInvariant(),
                        description = entry.Value.Description
                    })
                };

                return report.Status == HealthStatus.Healthy
                    ? Results.Ok(payload)
                    : Results.Json(payload, statusCode: StatusCodes.Status503ServiceUnavailable);
            });
        }
    }
}
