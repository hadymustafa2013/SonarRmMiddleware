using Data.Repository.CreditLens;
using log4net;
using Microsoft.AspNetCore.Mvc;
using RmMiddleware.CreditLensAggregator.Dto;
using RmMiddleware.CreditLensAggregator.Dto.Mapping;
using RmMiddleware.CreditLensAggregator.Dto.Spreading;
using Service = RmMiddleware.CreditLensApiAggregator.Service;

namespace RmMiddleware.Controllers.Api.CreditLensApiAggregator;

[Route("api/[controller]")]
public class Spreading(ILog log) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<SpreadingResponseDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ExceptionResponseDto>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpreadingResponseDto?>> Index([FromBody] SpreadingRequestDto? spreadingRequestDto)
    {
        var threadContextGuid = Guid.NewGuid().ToString();
        LogicalThreadContext.Properties["requestGuid"] = threadContextGuid;

        log.Info($"Request received.  Will proceed to validate the request.");

        try
        {
            if (spreadingRequestDto?.Search?.Entity is { Count: 0 })
                return BadRequest("Missing Entity Search Payload.");

            if (spreadingRequestDto?.FinancialTemplateMacros is { Count: 0 })
                return BadRequest("Missing Financial Template Macros in Payload.");

            if (spreadingRequestDto?.ModelRatingValueLookups is { Count: 0 })
                return BadRequest("Missing Model Rating Value Lookups in Payload.");

            log.Info($"Validation passed. Will map the entity search view from the request Dto.");

            var kvp = CreateEntitySearchViewCollection(spreadingRequestDto, log);

            log.Info($"Validation passed. Will map the entity search view from the request Dto.");

            var financialTemplateMacrosIds = UnpackFinancialTemplateMacrosIdsFromRequest(spreadingRequestDto, log);

            log.Info(
                $"Has concluded validation and unpacking of the payload values.  Will proceed to launch the spreading aggregation service.");

            var serviceProjectionSpreading = new Service.Spreading(
                new WeakWrapper(
                    new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
                    Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
                    Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin")
            );

            log.Info(
                $"Has created a Weak Wrapper instance with authentication headers which will be reused for CreditLens interactions.");

            var spreading =
                await serviceProjectionSpreading.GetProjectionSpreading(kvp, financialTemplateMacrosIds,
                    spreadingRequestDto?.Search?.Projection?.ProjectionTypes?.Select(s => s.Id).ToList(),
                    spreadingRequestDto?.Search?.Projection?.ScenarioTypes?.Select(s => s.Id).ToList()
                    , log);

            log.Info($"Has concluded the spreading aggregation.");

            if (spreading == null)
            {
                log.Info($"Spreading returned null.  Returning http status Not Found.");

                return NotFound();
            }

            log.Info($"Will proceed to map the spreading object to the view for return.");

            return MapSpreadingResponseDto.Map(spreadingRequestDto, spreading);
        }
        catch (Exception ex)
        {
            log.Error($"Exception {ex} returning 500.");
            
            return StatusCode(500);
        }
    }

    private static Dictionary<string, object> CreateEntitySearchViewCollection(SpreadingRequestDto? spreadingRequestDto,
        ILog log)
    {
        var kvp = new Dictionary<string, object>();
        if (spreadingRequestDto?.Search?.Entity == null) throw new Exception("Bad request. No search values.");

        foreach (var entitySearch in spreadingRequestDto.Search.Entity)
        {
            if (entitySearch.Key == null) continue;
            if (entitySearch.Value == null) continue;

            kvp.Add(entitySearch.Key, entitySearch.Value);

            log.Info($"Found key {entitySearch.Key} and value {entitySearch.Value}.  Added to the search payload.");
        }

        log.Info($"Returning {kvp.Count} for the search view.");

        return kvp;
    }

    private static List<int> UnpackFinancialTemplateMacrosIdsFromRequest(SpreadingRequestDto? spreadingRequestDto,
        ILog log)
    {
        var value = new List<int>();
        foreach (var macro in from financialTemplateMacro in spreadingRequestDto?.FinancialTemplateMacros
                 from macro in financialTemplateMacro.Macros
                 where !value.Contains(macro.Id)
                 select macro)
        {
            value.Add(macro.Id);
        }

        log.Info($"Has unpacked {value.Count} values from the FinancialTemplateMacros array element.");

        return value;
    }
}