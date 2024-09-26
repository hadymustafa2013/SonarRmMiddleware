using System.Reflection;
using Data.Repository;
using log4net;
using log4net.Config;
using Target = RmMiddleware.Bayan.Mapping;

namespace RmMiddleware.Tests.Bayan.Mapping;

public class AccountBalances
{
    [Fact]
    public async Task FetchMapAndAggregate()
    {
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        var log = LogManager.GetLogger(typeof(ILog));
        
        const int entityId = 1007;
        const string commercialRegistrationCode = "1010010813";
        const int filingId = 172748;
        
        var financial = await FinancialRepository.LookupFinancialIdGivenEntityIdAndPrimaryFlag(entityId);
        if (financial.FinancialTemplate == null) throw new Exception("Financial Not Found. Please check the Financial has been created and the template is MMAS.");

        var bayanSettings = await BayanSettingsRepository.LookupBayanSettingsFromFinancialTemplate(financial.FinancialTemplate,log);

        var mapping = new Target.Mapping(log);
        await mapping.FetchStatementMapAndAggregate(bayanSettings,entityId,entityId,commercialRegistrationCode,filingId);
    }
}