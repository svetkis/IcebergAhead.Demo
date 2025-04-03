using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IcebergAhead.Demo.HealthChecks;

public class ExternalAPIHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;

    public ExternalAPIHealthCheck(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/", cancellationToken);
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
