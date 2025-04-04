using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Polly;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using IcebergAhead.Demo.Services.DynamicService;
using IcebergAhead.Demo.Services;
using IcebergAhead.Demo.HealthChecks;
using IcebergAhead.Demo.FeatureFilters;
using IcebergAhead.Demo;
using IcebergAhead.Demo.Settings;
using IcebergAhead.Demo.Logging;

var builder = WebApplication.CreateBuilder(args);

//настраиваем логирование
builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration
       .Enrich.With<SensitiveDataMaskingEnricher>()
       .ReadFrom.Configuration(context.Configuration)
);

//подключаем телеметрию
builder.Services.AddTelemetry(builder.Configuration, "IcebergAhead.Demo");

builder.Services.AddHttpContextAccessor();

//подключаем Feature Management
builder.Services.AddScopedFeatureManagement()
        .AddFeatureFilter<BrowserFeatureFilter>();

//сервис для демонстрации динамической конфигурации
builder.Services.Configure<DynamicSettings>(builder.Configuration.GetSection("DynamicSettings"));
builder.Services.AddSingleton<IDynamicService, DynamicService>();

//регистрируем политики для retry, fallback, circuit breaker
builder.Services.AddResiliencePolicies();

// Добавляем конфигурацию в DI
builder.Services.Configure<IcebergAheadSettings>(builder.Configuration.GetSection(nameof(IcebergAheadSettings)));

// Добавляем демонстрационные сервисы в DI-контейнер
builder.Services.AddTransient<ITransientService, TransientService>();  // Transient
builder.Services.AddScoped<IScopedService, ScopedService>();           // Scoped
builder.Services.AddSingleton<ISingletonService, SingletonService>();  // Singleton

builder.Services.AddHttpClient("WithFallback", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
})
.AddPolicyHandlerFromRegistry("FallbackPolicy")
//.AddPolicyHandlerFromRegistry("RetryPolicy");//убрано для простоты демо, но в целом будет работать просто медленнее
.AddPolicyHandlerFromRegistry("CircuitBreakerPolicy");

//именной http клиент для контроллера
builder.Services.AddHttpClient("HttpBin", client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
    client.Timeout = TimeSpan.FromSeconds(3);
})
.AddPolicyHandlerFromRegistry((registry, request) =>
{
    return Policy.WrapAsync(
        registry.Get<IAsyncPolicy<HttpResponseMessage>>("RetryWithExponentialBackoff"),
        registry.Get<IAsyncPolicy<HttpResponseMessage>>("CircuitBreakerPolicy")
    );
});

//конфигурация клиента для HealthCheck по внешнему апи
builder.Services.Configure<ExternalApiOptions>(builder.Configuration.GetSection("HttpClients:ExternalAPI"));

//регистрируем типизированный HttpClient для ExternalAPIHealthCheck
builder.Services
    .AddHttpClient<ExternalAPIHealthCheck>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<ExternalApiOptions>>().Value;
        client.BaseAddress = new Uri(options.BaseAddress);
        client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
    });

builder.Services.AddHealthChecks()
    .AddCheck<ExternalAPIHealthCheck>("custom_health_check")
    // graceful shutdown health check, если получили сигнал о завершении, перестаем принимать запросы
    .AddCheck<GracefulShutdownHealthCheck>("graceful_shutdown");

//подключаем готовую либу healthchecks-ui
builder.Services.AddHealthChecksUI(setup =>
    {
        setup.AddHealthCheckEndpoint("self", "/healthz"); //имя и endpoint, который мы мониторим
    })
    .AddInMemoryStorage();  //обязательный storage для UI, можно и другие: PostgreSQL, SQL;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AcebergAhead API",
        Version = "v1",
        Description = "Best Resilence Practices API sample",
        Contact = new OpenApiContact
        {
            Name = "Svetlana Meleshkina",
            Email = "meleshkina_si@nlmk.com"
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<MaskingInLogsMiddleware>();

//логируем запросы из коробки без написания своей middleware
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Best Practices API v1");
        options.DisplayRequestDuration();
    });

    Log.Information("App started in development");
}

app.UseAuthorization();
app.MapControllers();

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("App is shutting down");
});

//маппинг healthchecks
app.MapHealthChecks("/health");

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// подключаем healthChecks UI
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";      // URL UI-панели
    options.ApiPath = "/health-ui-api"; // API для UI
});

app.Run();
