using Data.Repository;
using Microsoft.AspNetCore.Mvc;

namespace RmMiddleware.Controllers.Api.Bayan;

[Route("api/Bayan/[controller]")]
public class Pdf : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Index(int entityId, int filingId)
    {
        try
        {
            var search = new Dictionary<string, object>
            {
                { "FilingId", filingId },
                { "EntityId", entityId }
            };

            var bayanStatements = await BayanStatementRepository.Search(search);

            if (bayanStatements.Count == 0) throw new Exception("Not Found.");

            var bayanStatement = bayanStatements.FirstOrDefault();

            if (bayanStatement?.Pdf == null) throw new Exception("Not Found.");

            var bytes = Convert.FromBase64String(bayanStatement.Pdf);
            return File(bytes, "application/pdf");
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}