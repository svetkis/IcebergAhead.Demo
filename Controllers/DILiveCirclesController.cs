using IcebergAhead.Demo.Services;
using IcebergAhead.Demo.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

[Route("api/[controller]")]
[ApiController]
public class DILiveCirclesController : ControllerBase
{
    private readonly ITransientService _transient;
    private readonly IScopedService _scoped;
    private readonly ISingletonService _singleton;
    private readonly ILogger<DILiveCirclesController> _logger;
    private readonly IcebergAheadSettings _settings;

    public DILiveCirclesController(
        ITransientService transient,
        IScopedService scoped,
        ISingletonService singleton,
        ILogger<DILiveCirclesController> logger,
        IOptions<IcebergAheadSettings> options)
    {
        _transient = transient;
        _scoped = scoped;
        _singleton = singleton;
        _logger = logger;
        _settings = options.Value;
    }

    [HttpGet("settings")]
    public IActionResult GetSettings()
    {
        return Ok(new
        {
            ShipName = _settings.ShipName,
            ResilienceLevel = _settings.ResilienceLevel
        });
    }

    //не рекомендуется использовать serviceProvider, это сделано для упрощения демонстрации scope
    [HttpGet]
    public IActionResult Get([FromServices] IServiceProvider serviceProvider)
    {
        return Ok(new
        {
            DirectInjection = new
            {
                Transient = _transient.GetGuid(),
                Scoped = _scoped.GetGuid(),
                Singleton = _singleton.GetGuid()
            },
            ResolvedViaProvider = new
            {
                Transient1 = serviceProvider.GetRequiredService<ITransientService>().GetGuid(),
                Transient2 = serviceProvider.GetRequiredService<ITransientService>().GetGuid(),
                Scoped1 = serviceProvider.GetRequiredService<IScopedService>().GetGuid(),
                Scoped2 = serviceProvider.GetRequiredService<IScopedService>().GetGuid(),
                Singleton1 = serviceProvider.GetRequiredService<ISingletonService>().GetGuid(),
                Singleton2 = serviceProvider.GetRequiredService<ISingletonService>().GetGuid()
            }
        });
    }

    [HttpGet("scoped-test")]
    public IActionResult ScopedTest([FromServices] IServiceProvider rootProvider)
    {
        // Обычное получение из scope запроса
        var scoped1 = rootProvider.GetRequiredService<IScopedService>();

        // Вложенный scope, явно создаём
        using var newScope = rootProvider.CreateScope();
        var scoped2 = newScope.ServiceProvider.GetRequiredService<IScopedService>();

        return Ok(new
        {
            FromHttpRequestScope = scoped1.GetGuid(),
            FromNewCreatedScope = scoped2.GetGuid(),
            SameInstance = scoped1.GetGuid() == scoped2.GetGuid()
        });
    }
}
