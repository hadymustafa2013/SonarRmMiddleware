using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace RmMiddleware.Controllers.BayanTests;

[Route("[controller]")]
public class GetJsonWebTokenController : ControllerBase
{
    [HttpGet]
    public FileContentResult Index()
    {
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "wwwroot", "GetJsonWebToken.json");
        var returnFile = System.IO.File.ReadAllBytes(path);
        return new FileContentResult(returnFile, "application/json");
    }
}