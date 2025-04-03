namespace IcebergAhead.Demo.Settings;

public class ExternalApiOptions
{
    public string BaseAddress { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 5;
    public string UserAgent { get; set; } = "DemoApp";
}
