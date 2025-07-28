using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IcebergAhead.Demo.HealthChecks;

public class GracefulShutdownHealthCheck : IHealthCheck
{
    private readonly IHostApplicationLifetime _hostAppLifetime;

    public GracefulShutdownHealthCheck(IHostApplicationLifetime lifetime)
    {
        _hostAppLifetime = lifetime;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Если приложение в состоянии остановки — выдаём Unhealthy
        if (_hostAppLifetime.ApplicationStopping.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy("App is stopping");
        }

        return HealthCheckResult.Healthy("App is running");
    }
}
