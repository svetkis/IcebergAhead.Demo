using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace IcebergAhead.Demo.Controllers;

//только для видимости через сваггер, вообще контроллер не нужен, хелфчеки будут работать и так
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    [HttpGet]
    [Route("/healthz")]
    public async Task<IActionResult> Get()
    {
        var report = await _healthCheckService.CheckHealthAsync();
        if (report.Status == HealthStatus.Healthy)
            return Ok(new { status = "Healthy" });

        return StatusCode(503, new { status = "Unhealthy" });
    }
}
