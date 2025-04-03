using Microsoft.AspNetCore.Mvc;

namespace IcebergAhead.Demo.Controllers;

[ApiController]
[Route("api/fault-tolerance")]
public class FaultToleranceController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FaultToleranceController> _logger;

    public FaultToleranceController(
        IHttpClientFactory httpClientFactory,
        ILogger<FaultToleranceController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpGet("fallback")]
    public async Task<IActionResult> GetWithFallback()
    {
        _logger.LogInformation("Sending request that triggers fallback");

        var client = _httpClientFactory.CreateClient("WithFallback");

        try
        {
            var response = await client.GetAsync("status/500");
            var result = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Received response. StatusCode: {StatusCode}, Body: {Content}",
                response.StatusCode, result);

            return Content(result, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fallback failed to handle the error");
            return StatusCode(500, "Critical error — fallback did not resolve the issue.");
        }
    }

    [HttpGet("delay")]
    public async Task<IActionResult> GetDelayedResponse()
    {
        var url = "delay/5";
        _logger.LogInformation("Sending delayed request to: {Url}", url);

        try
        {
            var client = _httpClientFactory.CreateClient("HttpBin");
            var response = await client.GetAsync(url);
            return Ok(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during delayed response");
            return StatusCode(500, "Request timeout or failure.");
        }
    }

    [HttpGet("error")]
    public async Task<IActionResult> GetErrorResponse()
    {
        var url = "status/500";
        _logger.LogInformation("Sending predefined error request to: {Url}", url);

        try
        {
            var client = _httpClientFactory.CreateClient("HttpBin");
            var response = await client.GetAsync(url);
            return Ok(await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Simulated HTTP 500 error handled");
            return StatusCode(500, "Simulated server-side error response.");
        }
    }
}
