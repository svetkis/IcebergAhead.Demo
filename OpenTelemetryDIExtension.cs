using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace IcebergAhead.Demo;

public static class OpenTelemetryDIExtension
{
    public static IServiceCollection AddTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        string appName)
    {
        //services.Configure<JaegerExporterOptions>(configuration.GetSection(nameof(JaegerExporterOptions)));

        string podName = Environment.GetEnvironmentVariable("POD_NAME") ?? Environment.MachineName;
        string aspnet_environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown";

        services
            .AddOpenTelemetry()
                .ConfigureResource(b =>
                {
                    b.AddService(appName);

                    // обогащаем данные
                    b.AddAttributes(
                    [
                        new KeyValuePair<string, object>("instance.id", podName),
                        new KeyValuePair<string, object>("environment", aspnet_environment)
                    ]);
                })
                .WithTracing(
                    b => b
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        //.AddConsoleExporter() //просто для демо
                        //.AddJaegerExporter() //закомменитировано потому что не подключен приемник телеметрии
                        )
                .WithLogging(
                    _ => { },
                    o =>
                    {
                        o.IncludeFormattedMessage = true;
                        o.AddConsoleExporter();
                    });
        ;

        return services;
    }
}
