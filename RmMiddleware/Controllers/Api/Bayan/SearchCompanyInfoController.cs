using Data.Repository;
using Data.Repository.Dto;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RmMiddleware.Bayan;

namespace RmMiddleware.Controllers.Api.Bayan;

[Route("api/Bayan/[controller]")]
public class SearchCompanyInfoController(ILog log) : ControllerBase
{
    [HttpPut]
    public async Task<ActionResult<BayanStatementRequestDto>> Index(int entityId)
    {
        try
        {
            var threadContextGuid = Guid.NewGuid().ToString();
            LogicalThreadContext.Properties["requestGuid"] = threadContextGuid;
            
            log.Info($"Setting thread context RequestGuid:{threadContextGuid}.");
            
            log.Info(
                $"New message on SearchCompanyInfo Controller for Entity Id {entityId}. " +
                $"Will proceed to Lookup Financial Id Given Entity Id And Primary Flag from static repository.");

            var financial = await FinancialRepository.LookupFinancialIdGivenEntityIdAndPrimaryFlag(entityId);

            log.Info(
                $"Concluded Lookup Financial Id Given Entity Id And Primary Flag for Entity Id {entityId}.  Financial Id {financial.Id} found.");

            if (financial.FinancialTemplate == null)
            {
                log.Info(
                    $"Lookup Financial Id Given Entity Id And Primary Flag for Entity Id {entityId} " +
                    $"has returned null and will throw an exception.");

                throw new Exception("Financial not found.  Please check the Financial has been created and the template is MMAS.");
            }

            log.Info(
                $"Lookup Financial Template ID {financial.Id} and Financial Template {financial.FinancialTemplate} " +
                $"Id Given Entity Id And Primary Flag for Entity Id {entityId} " +
                $"has returned a value and will and Lookup Bayan Settings From Financial Template from repository.");

            var bayanSettings =
                await BayanSettingsRepository.LookupBayanSettingsFromFinancialTemplate(financial.FinancialTemplate, log);

            log.Info(
                $"For Entity Id {entityId} Lookup Financial Id Given Entity Id And Primary Flag for Entity Id {entityId} " +
                $"has returned a value and will and Lookup Bayan Settings From Financial Template {financial.FinancialTemplate}.");
            
            log.Debug($"BayanSettings: {JsonConvert.SerializeObject(bayanSettings)}");
            
            log.Info($"For Entity Id {entityId} is going to new up the Integration for connection to Bayan.");

            var integration = new Integration(
                new Uri(bayanSettings.BayanHttpEndpointJwt ?? "http://localhost:5079"),
                new Uri(bayanSettings.BayanHttpEndpointStatementList ?? "http://localhost:5079/CommercialCreditReport/1.0/"),
                bayanSettings.BayanHttpUser ?? "admin",
                bayanSettings.BayanHttpPassword ?? "admin", log,
                bayanSettings.BayanAuthentication,bayanSettings.BayanChannelIdHeader);

            log.Info(
                $"For Entity Id {entityId} and Financial Template Id {financial.FinancialTemplate} " +
                $"has instantiated the Bayan class.  " +
                $"Will invoke its Search Company Info method.");

            return await integration.SearchCompanyInfo(entityId);
        }
        catch (Exception ex)
        {
            log.Error($"Controller has created error as {ex} and will return Bad Request with message value.");

            return BadRequest(ex.Message);
        }
    }
}