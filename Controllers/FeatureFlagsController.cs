using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace IcebergAhead.Demo.Controllers;

[ApiController]
[Route("api/feature-flags")]
public class FeatureFlagsController : ControllerBase
{
    private readonly IFeatureManager _featureManager;
    private readonly ILogger<FeatureFlagsController> _logger;

    public FeatureFlagsController(IFeatureManager featureManager, ILogger<FeatureFlagsController> logger)
    {
        _featureManager = featureManager;
        _logger = logger;
    }

    [HttpGet("new-feature")]
    public async Task<IActionResult> GetFeatureFlag()
    {
        var isEnabled = await _featureManager.IsEnabledAsync("NewFeature2");

        _logger.LogInformation("Feature 'NewFeature2' status: {Status}", isEnabled ? "ENABLED" : "DISABLED");

        return Ok($"NewFeature2 is {(isEnabled ? "ENABLED" : "DISABLED")}");
    }

    [HttpGet("by-browser")]
    public async Task<IActionResult> GetUIType()
    {
        var isEnabled = await _featureManager.IsEnabledAsync("NewUIByBrowser");

        _logger.LogInformation("Feature 'NewUIByBrowser' status: {Status}", isEnabled ? "ENABLED" : "DISABLED");

        return Ok($"NewUIByBrowser is {(isEnabled ? "ENABLED" : "DISABLED")}");
    }
}

