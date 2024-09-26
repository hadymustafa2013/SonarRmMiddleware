using System.Reflection;
using log4net;
using log4net.Config;
using Spreading = RmMiddleware.Bayan.Mapping.Spreading;

namespace RmMiddleware.Tests.Bayan.Mapping;

public class FinancialSpreading
{
    [Fact]
    public async Task FetchMapAndAggregate()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        var log = LogManager.GetLogger(typeof(ILog));
        
        var financialSpreading = new Spreading(log);
        await financialSpreading.CreateFinancialStatementFromBayanFilingId(10007,  172748);
    }
}