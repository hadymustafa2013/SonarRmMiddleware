using System.Reflection;
using log4net;
using log4net.Config;
using Target = RmMiddleware.Bayan;

namespace RmMiddleware.Tests.Bayan;

public class IntegrationTests
{
    [Fact]
    public async Task GetSearchCompanyInfo()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        var log = LogManager.GetLogger(typeof(ILog));
        
        var integration = new Target.Integration(
            new Uri(Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_JWT") ?? "https://api.bayancb.com/JWTToken/1.0/"),
            new Uri(Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_REPORT") ?? "https://api.bayancb.com/CommercialCreditReport/1.0/"),
            Environment.GetEnvironmentVariable("BAYAN_HTTP_USER") ?? "RAJHITST",
            Environment.GetEnvironmentVariable("BAYAN_HTTP_PASSWORD") ?? "Password1!",log,true,false);
        var companyInfo = await integration.SearchCompanyInfo(1010010813);
        Assert.True(companyInfo != null);
    }

    [Fact]
    public async Task GetStatementByFinancialId()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        var log = LogManager.GetLogger(typeof(ILog));
        
        var integration = new Target.Integration(
            new Uri(Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_JWT") ?? "https://api.bayancb.com/JWTToken/1.0/"),
            new Uri(Environment.GetEnvironmentVariable("BAYAN_HTTP_ENDPOINT_REPORT") ?? "https://api.bayancb.com/CommercialCreditReport/1.0/"),
            Environment.GetEnvironmentVariable("BAYAN_HTTP_USER") ?? "RAJHITST",
            Environment.GetEnvironmentVariable("BAYAN_HTTP_PASSWORD") ?? "Password1!",log,true,false);
        var financialSummary = await integration.GetStatementByFinancialId("1010010813",172428,log);
        Assert.True(financialSummary != null);
    }
}