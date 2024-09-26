using Data.Repository.CreditLens;
using Newtonsoft.Json;

var wrapper = new WeakWrapper(
    new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
    Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
    Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");
var lines = File.ReadAllLines(@"C:\Users\rchurchman\Downloads\Staging.csv");

var headers = new List<string>();
for (var i = 0; i < lines.Length; i++)
{
    var splits = lines[i].Split(",");

    if (i == 0)
    {
        headers.AddRange(splits);
        continue;
    }

    Console.WriteLine($"Read line: {lines[i]}");

    try
    {
        var payload = new Dictionary<string, string> { { "Id", i.ToString() } };
        for (var j = 0; j < splits.Length; j++) payload.Add(headers[j], splits[j]);
        payload.Add("OperationType", "Create");

        if (Validate(payload))
            try
            {
                var payloadString = JsonConvert.SerializeObject(payload);
                Console.WriteLine($"About to POST: {payloadString}");
                var response = await wrapper.PostReturnJObject("/api/refData/SourceTargetAccountMapping", "[" + payloadString + "]");
                Console.WriteLine($"Response {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in line {i} as {ex}");
            }
        else
            Console.WriteLine($"Validation failed for {i}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception in line {i} as {ex}.");
    }
}

return;

bool Validate(Dictionary<string, string> payload)
{
    if (string.IsNullOrEmpty(payload["TargetFinancialTemplateId"])) return false;
    if (!int.TryParse(payload["TargetFinancialTemplateId"], out var _)) return false;

    if (string.IsNullOrEmpty(payload["SourceCoefficient"])) payload["SourceCoefficient"] = "1";

    if (string.IsNullOrEmpty(payload["EvaluationPriority"])) payload["EvaluationPriority"] = "1";

    return payload["SourceDocCode"] switch
    {
        "3976" => true,
        "3977" => true,
        "3984" => true,
        "3985" => true,
        _ => false
    };
}