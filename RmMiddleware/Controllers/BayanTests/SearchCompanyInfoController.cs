using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace RmMiddleware.Controllers.BayanTests;

[Route("CommercialCreditReport/1.0/[controller]")]
public class SearchCompanyInfoController : ControllerBase
{
    [HttpGet]
    // ReSharper disable once UnusedParameter.Global
    public FileContentResult Index(string companyId)
    {
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "wwwroot", "SearchCompanyInfo.json");
        var returnFile = System.IO.File.ReadAllBytes(path);
        return new FileContentResult(returnFile, "application/json");
    }
}