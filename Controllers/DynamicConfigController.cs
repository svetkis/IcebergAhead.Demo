using Microsoft.AspNetCore.Mvc;
using IcebergAhead.Demo.Services.DynamicService;

[ApiController]
[Route("api/[controller]")]
public class DynamicConfigController : ControllerBase
{
    private readonly IDynamicService _service;

    public DynamicConfigController(IDynamicService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_service.GetStatus());
    }
}
