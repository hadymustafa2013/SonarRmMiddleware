using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RmMiddleware.Bayan.Mapping;
using RmMiddleware.Models;

namespace RmMiddleware.Controllers.Api.Bayan;

[Route("api/Bayan/[controller]")]
public class SpreadBayanFinancialStatementController(ILog log) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Index(
        [FromBody] SpreadBayanFinancialStatementRequest spreadBayanFinancialStatementRequest)
    {
        try
        {
            var threadContextGuid = Guid.NewGuid().ToString();
            LogicalThreadContext.Properties["requestGuid"] = threadContextGuid;
            
            log.Info($"Setting thread context RequestGuid:{threadContextGuid}.");
            
            log.Info($"SpreadBayanFinancialStatementRequest Controller invoked with payload " +
                     $"{JsonConvert.SerializeObject(spreadBayanFinancialStatementRequest)}.");
        
            log.Info("Will instantiate the Spreading object and launch the method " +
                     "to Create Financial Statement From Bayan Filing Id.");
        
            var financialSpreading = new Spreading(log);
            await financialSpreading
                .CreateFinancialStatementFromBayanFilingId(spreadBayanFinancialStatementRequest.ModelId,
                    spreadBayanFinancialStatementRequest.FilingId);
        
            log.Info("Concluded Create Financial Statement From Bayan Filing Id and will return OK.");
        
            return Ok();
        }
        catch (Exception ex)
        {
            log.Error($"Controller has created error as {ex} and will return Bad Request with message value.");

            return BadRequest(ex.Message);
        }
    }
}