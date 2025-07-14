using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IcebergAhead.Demo.HealthChecks;

public class ExternalAPIHealthCheck(HttpClient httpClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await httpClient.GetAsync("/", cancellationToken);
            if (response.IsSuccessStatusCode)
                return HealthCheckResult.Healthy("External API is reachable");

            return HealthCheckResult.Degraded("External API has some problems");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("External API is down!");
        }
    }
}
