using Microsoft.FeatureManagement;

namespace IcebergAhead.Demo.FeatureFilters;

[FilterAlias("Browser")]
public class BrowserFeatureFilter : IFeatureFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BrowserFeatureFilter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
    {
        var parameters = context.Parameters.Get<BrowserFeatureFilterSettings>();
        var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();

        if (string.IsNullOrEmpty(userAgent))
        {
            return Task.FromResult(false);
        }

        // Проверяем, какой браузер используется
        bool isEdge = userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase); // Edge
        bool isChrome = userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase); // Chrome

        // Сравнение с разрешенными браузерами
        bool isAllowedBrowser = parameters.AllowedBrowsers.Any(browser =>
            browser == "Edge" && isEdge ||
            browser == "Chrome" && isChrome);

        return Task.FromResult(isAllowedBrowser);
    }
}
