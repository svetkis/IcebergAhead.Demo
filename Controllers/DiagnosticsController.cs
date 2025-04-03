using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IcebergAhead.Demo.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(
        IHttpClientFactory httpClientFactory,
        ILogger<DiagnosticsController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("ip")]
    public async Task<IActionResult> GetMyIp()
    {
        const string url = "ip";
        _logger.LogInformation("Requesting IP address from: {Url}", url);

        try
        {
            var client = _httpClientFactory.CreateClient("HttpBin");
            var result = await client.GetStringAsync(url);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve IP address");
            return StatusCode(500, "IP lookup failed");
        }
    }

    [HttpGet("trace-log")]
    public IActionResult LogTrace()
    {
        _logger.LogInformation("Trace log emitted — TraceId: {TraceId}, SpanId: {SpanId}, {email}, {password}, {phone}",
            Activity.Current?.TraceId.ToString(),
            Activity.Current?.SpanId.ToString(),
            "user_us@nlmk.com",
            "veryStupidToPlacePasswordHere",
            "+79001234567");

        return Ok("Logger invoked with TraceId and SpanId.");
    }
}
