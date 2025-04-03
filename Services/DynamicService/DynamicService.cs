using IcebergAhead.Demo.Settings;
using Microsoft.Extensions.Options;

namespace IcebergAhead.Demo.Services.DynamicService;

public class DynamicService : IDynamicService
{
    private readonly IOptionsMonitor<DynamicSettings> _monitor;
    private DynamicSettings _cached;

    public DynamicService(IOptionsMonitor<DynamicSettings> monitor)
    {
        _monitor = monitor;
        _cached = monitor.CurrentValue;

        // Подписка на изменение на лету
        _monitor.OnChange(newValue =>
        {
            Console.WriteLine($"[CONFIG] Dynamic setting changed: MaxItems={newValue.MaxItems}, FeatureEnabled={newValue.FeatureEnabled}");
            _cached = newValue; // можно кэшировать или пересоздавать что-то
        });
    }

    public string GetStatus()
    {
        // Альтернатива без кэша: _monitor.CurrentValue.FeatureEnabled
        return $"Feature = {_cached.FeatureEnabled}, MaxItems = {_cached.MaxItems}";
    }
}
