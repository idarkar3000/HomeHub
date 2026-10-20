namespace HomeHub.Endpoints
{
    public static class HealthEndpoints
    {
        // GET y HEAD porque UptimeRobot hace HEAD y Render hace GET para mantener el servicio despierto
        public static void MapHealthEndpoints(this WebApplication app)
        {
            app.MapMethods("/", ["GET", "HEAD"], () => Results.Ok(new { status = "healthy", app = "HomeHub" }));
            app.MapMethods("/health", ["GET", "HEAD"], () => Results.Ok(new { status = "healthy", time = DateTime.UtcNow }));
        }
    }
}
