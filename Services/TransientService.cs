namespace IcebergAhead.Demo.Services;

public class TransientService : ITransientService
{
    private readonly string _guid = Guid.NewGuid().ToString();
    public string GetGuid() => _guid;
}
