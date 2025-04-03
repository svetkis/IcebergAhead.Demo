namespace IcebergAhead.Demo.Services;

public class ScopedService : IScopedService
{
    private readonly string _guid = Guid.NewGuid().ToString();
    public string GetGuid() => _guid;
}