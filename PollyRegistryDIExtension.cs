using Polly.Extensions.Http;
using Polly.Registry;
using Polly;
using Polly.CircuitBreaker;

namespace IcebergAhead.Demo;

public static class PollyRegistryDIExtension
{
    public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
    {
        var registry = new PolicyRegistry();

        IAsyncPolicy<HttpResponseMessage> retryWithExponentialBackoff = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30));

        IAsyncPolicy<HttpResponseMessage> fallbackPolicy = Policy<HttpResponseMessage>
            .HandleResult(r => !r.IsSuccessStatusCode) // Fallback if response is not successful
            .Or<BrokenCircuitException>() // Fallback if circuit breaker is open
            .FallbackAsync(
                fallbackAction: (delegateResult, ct) =>
                {
                    var fallbackResponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StringContent(
                            """{ "message": "Fallback triggered" }""",
                            System.Text.Encoding.UTF8,
                            "application/json")
                    };

                    return Task.FromResult(fallbackResponse);
                },
                onFallbackAsync: (outcome, context) =>
                {
                    Console.WriteLine($"[Fallback] Triggered. Reason: {outcome.Result?.StatusCode}");
                    return Task.CompletedTask;
                }
            );

        registry.Add("RetryWithExponentialBackoff", retryWithExponentialBackoff);
        registry.Add("CircuitBreakerPolicy", circuitBreakerPolicy);
        registry.Add("FallbackPolicy", fallbackPolicy);

        services.AddSingleton<IPolicyRegistry<string>>(registry);
        services.AddSingleton<IReadOnlyPolicyRegistry<string>>(registry);

        return services;
    }
}
