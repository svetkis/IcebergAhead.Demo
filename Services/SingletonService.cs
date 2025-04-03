namespace IcebergAhead.Demo.Services;

public class SingletonService : ISingletonService
{
    private readonly string _guid = Guid.NewGuid().ToString();
    public string GetGuid() => _guid;
}