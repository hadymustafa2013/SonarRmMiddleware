using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace RmMiddleware.Controllers.BayanTests;

[Route("CommercialCreditReport/1.0")]
public class CommercialCreditReportController : ControllerBase
{
    [HttpPost]
    [Route("CB_ME_Product")]
    public async Task<FileContentResult> Index()
    {
        var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms);
        var jObject = JObject.Parse(Encoding.UTF8.GetString(ms.ToArray()));
        var financialId = jObject["Product"]?[0]?["CB_ME_ProductInput"]?["B2BInfo"]?["Financial"]?[0]?["FilingID"];
        
        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            "wwwroot", $"CbMeProduct/{financialId}.json");
        var returnFile = await System.IO.File.ReadAllBytesAsync(path);
        return new FileContentResult(returnFile, "application/json");
    }
}