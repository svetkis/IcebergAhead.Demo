using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IcebergAhead.Demo.Controllers;

[ApiController]
[Route("api/diagnostics")]
public class DiagnosticsController(
    IHttpClientFactory httpClientFactory,
    ILogger<DiagnosticsController> logger) : ControllerBase
{
    [HttpGet("ip")]
    public async Task<IActionResult> GetMyIp()
    {
        const string url = "ip";
        logger.LogInformation("Requesting IP address from: {Url}", url);

        try
        {
            var client = httpClientFactory.CreateClient("HttpBin");
            var result = await client.GetStringAsync(url);

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve IP address");
            return StatusCode(500, "IP lookup failed");
        }
    }
}

