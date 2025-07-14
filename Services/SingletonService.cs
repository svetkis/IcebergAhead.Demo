namespace IcebergAhead.Demo.Services;

public class SingletonService(ILogger<SingletonService> logger) : ISingletonService
{
    private readonly string _guid = Guid.NewGuid().ToString();
    public string GetGuid()
    {
        logger.LogInformation("Structured log. Returned {Guid}. Phone {Phone}",  _guid, "+7 111 111 11 11");
        logger.LogInformation($"Unstructured log. Returned {_guid} +7 111 111 11 11");

        return _guid;
    }
}