using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IcebergAhead.Demo.HealthChecks;

public class GracefulShutdownHealthCheck : IHealthCheck
{
    private readonly IHostApplicationLifetime _lifetime;

    public GracefulShutdownHealthCheck(IHostApplicationLifetime lifetime)
    {
        _lifetime = lifetime;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        // Если приложение уже в состоянии остановки — выдаём Unhealthy
        if (_lifetime.ApplicationStopping.IsCancellationRequested)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy("App is stopping"));
        }

        return Task.FromResult(HealthCheckResult.Healthy("App is running"));
    }
}
