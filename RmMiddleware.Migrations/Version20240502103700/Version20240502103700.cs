using System.Reflection;
using Data.Repository.CreditLens;
using Data.Repository.POCO;
using FluentMigrator;
using Attribute = Data.Repository.CreditLens.Models.Model.Attribute;

namespace RmMiddleware.Migrations.Version20240502103700;

[Migration(20240502103700)]
public class Version20240502103700 : Migration
{
    public override void Up()
    {
        var authenticationHeader = BayanStatementRequest();
        CreateBayanStatement(authenticationHeader);
        // CreateBayanStatementRequestViewResources(authenticationHeader);
        // CreateFinancialAnalysisResources(authenticationHeader);
        // CreateBayanStatementViewResources(authenticationHeader);
        CreateBayanStatementView(authenticationHeader);
        CreateBayanStatementRequestView(authenticationHeader);
        CreateDisplayAllBusinessRule(authenticationHeader);
        CreateFinancialAnalysisMenuItem(authenticationHeader);
        CreateSourceTargetAccountMapping(authenticationHeader);
        CreateFinancialSearchView(authenticationHeader);
        CreateBayanSettings(authenticationHeader);
        CreateBayanConstant(authenticationHeader);
        CreateConsentInExistingEntityModelAndView(authenticationHeader);
        CreateBayanSettingsSearchView(authenticationHeader);
        CreateBayanSettingsEditView(authenticationHeader);
        CreateBayanConstantsView(authenticationHeader);
        CreateDefaultMenu(authenticationHeader);
    }

    private static void CreateBayanStatementView(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanStatementView.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly($"/api/meta/v1/modules/Tenant/viewModels",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();

        var taskWeakPut = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly(
                $"/api/meta/v1/modules/Tenant/viewModels/BayanStatementView/versions/1/activate", null
            );
        });
        taskWeakPut.Wait();
    }

    private static void CreateConsentInExistingEntityModelAndView(Dictionary<string, string?>? authenticationHeader)
    {
        var attributeModel = CreateConsentAttributeModel();

        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var taskWeakModel = Task.Run(async () => { await helpersWeak.AddAttributeToModel("Entity", attributeModel); });
        taskWeakModel.Wait();

        var attributeView = CreateConsentAttributeView(attributeModel);

        var taskWeakView = Task.Run(async () =>
        {
            await helpersWeak.AddAttributeToView("EntityEdit", 0, attributeView,0);
        });
        taskWeakView.Wait();

        var taskWeakSearchViewSearchGroup = Task.Run(async () =>
        {
            await helpersWeak.AddAttributeToView("EntitySearch", 0, attributeView);
        });
        taskWeakSearchViewSearchGroup.Wait();

        var taskWeakSearchViewResultGroup = Task.Run(async () =>
        {
            await helpersWeak.AddAttributeToView("EntitySearch", 1, attributeView);
        });
        taskWeakSearchViewResultGroup.Wait();
    }

    private static Data.Repository.CreditLens.Models.View.Attribute CreateConsentAttributeView(Attribute attributeModel)
    {
        var attributeView = new Data.Repository.CreditLens.Models.View.Attribute()
        {
            Id = attributeModel.Id,
            PrimaryField = false,
            ViewModelRefSpecified = false,
            Validator = null,
            RunRuleSpecified = false,
            DependsOnAttributesSpecified = false,
            Width = null,
            Filter = false,
            Hidden = false,
            ResourceId = null,
            CellIcon = null,
            CellTooltip = null,
            HorizontalAlign = null,
            ReadOnly = false,
            ModelId = "Entity",
            ModelFieldId = attributeModel.Id,
            Expression = null,
            Encrypt = false,
            ControlType = "Checkbox",
            LookupDependency = null,
            AllowManualEdit = null,
            Format = null,
            Decimals = null,
            Rounding = null,
            ParentAttributeId = null,
            VmRefId = null,
            DisplayField = false,
            MultiSelectField = false,
            LeafOnly = true,
            Action = null,
            ActionRule = null,
            ActionConfigSetting = null,
            DefaultValue = null,
            ViewLabelFieldId = null,
            NotesEnabled = false,
            Deleted = false,
            ChildModelIdFilter = null,
            VmRefParam = null,
            SortBy = null,
            Shared = true,
            Editable = true,
            Sortable = true,
            Resizeable = true,
            WrapHeaderDisplay = false,
            PinColumnToLeft = false,
            IsAuthAttribute = false,
            SetDefaultOnly = false,
            LinkType = null,
            LinkCondition = null,
            TargetType = null,
            TargetViewModelId = null,
            TargetGroupId = null,
            TargetRouteId = null,
            ModalTitle = null,
            RefParam = null,
            DisplayAsKey = false,
            ClarifyTextEnabled = false,
            CloudBehavior = false,
            ResourceModuleId = "Tenant"
        };
        return attributeView;
    }

    private static Attribute CreateConsentAttributeModel()
    {
        var attributeModel = new Data.Repository.CreditLens.Models.Model.Attribute()
        {
            Id = "Consent",
            AttributeExpression = null,
            Calculated = false,
            Encrypt = false,
            Description = "Consent",
            IsNew = true,
            RefAttribute = [],
            RefModelId = null,
            ResourceLabel = "Consent",
            Transient = null,
            Unique = null,
            PrimaryField = false,
            DataType = "Boolean"
        };
        return attributeModel;
    }

    private static void CreateSourceTargetAccountMapping(Dictionary<string, string?>? headers)
    {
        var sourceTargetAccountMapping = new SourceTargetAccountMapping();
        var helpersTypedBayanStatement = new TypedWrapper<SourceTargetAccountMapping>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            headers);

        var taskTypedBayanStatementRequest = Task.Run(async () =>
        {
            await helpersTypedBayanStatement.BuildReferenceData(sourceTargetAccountMapping);
        });
        taskTypedBayanStatementRequest.Wait();
    }

    private static void CreateBayanConstant(Dictionary<string, string?>? headers)
    {
        var bayanConstant = new BayanConstant();
        var helpersTypedBayanStatement = new TypedWrapper<BayanConstant>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            headers);

        var taskTypedBayanStatementRequest = Task.Run(async () =>
        {
            await helpersTypedBayanStatement.BuildModelAndViews(bayanConstant);
        });
        taskTypedBayanStatementRequest.Wait();
    }

    private static void CreateBayanStatementRequestViewResources(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanStatementRequestViewResources.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly("/api/i18n/v1/resource/Tenant_en/viewModel/BayanStatementRequestView",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();
    }

    private static void CreateFinancialAnalysisResources(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "FinancialAnalysisResources.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly("/api/i18n/v1/resource/Tenant_en/menu/FinancialAnalysis",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();
    }

    private static void CreateBayanStatementViewResources(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanStatementViewResources.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly("/api/i18n/v1/resource/Tenant_en/viewModel/BayanStatementView",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();
    }

    private static void CreateDisplayAllBusinessRule(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "DisplayTrue.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly("api/code/rules/save",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();
    }

    private static void CreateFinancialAnalysisMenuItem(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "FinancialAnalysisMenu.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly($"/api/meta/v1/modules/Tenant/menus/FinancialAnalysis",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();
    }

    private static void CreateBayanStatementRequestView(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanStatementRequestView.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly($"/api/meta/v1/modules/Tenant/viewModels",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();

        var taskWeakPut = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly(
                $"/api/meta/v1/modules/Tenant/viewModels/BayanStatementRequestView/versions/1/activate", null
            );
        });
        taskWeakPut.Wait();
    }
    
    private static void CreateBayanSettingsSearchView(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanSettingsSearchView.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly($"/api/meta/v1/modules/Tenant/viewModels",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();

        var taskWeakPut = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly(
                $"/api/meta/v1/modules/Tenant/viewModels/BayanSettingsSearchView/versions/1/activate", null
            );
        });
        taskWeakPut.Wait();
    }
    
    private static void CreateBayanSettingsEditView(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanSettingsEditView.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly($"/api/meta/v1/modules/Tenant/viewModels",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();

        var taskWeakPut = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly(
                $"/api/meta/v1/modules/Tenant/viewModels/BayanSettingsEditView/versions/1/activate", null
            );
        });
        taskWeakPut.Wait();
    }
    
    private static void CreateBayanConstantsView(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "BayanConstantView.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly($"/api/meta/v1/modules/Tenant/viewModels",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();

        var taskWeakPut = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly(
                $"/api/meta/v1/modules/Tenant/viewModels/BayanConstantView/versions/1/activate", null
            );
        });
        taskWeakPut.Wait();
    }

    private static void CreateDefaultMenu(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "Menu.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly($"/api/meta/v1/modules/Tenant/menus/Default",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();
    }
    
    private static void CreateFinancialSearchView(Dictionary<string, string?>? authenticationHeader)
    {
        var helpersWeak = new WeakWrapper(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            authenticationHeader);

        var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
            @"Version20240502103700", "FinancialSearch.json");

        var taskWeak = Task.Run(async () =>
        {
            await helpersWeak.PostStringBodyOnly($"/api/meta/v1/modules/Tenant/viewModels",
                await File.ReadAllTextAsync(path)
            );
        });
        taskWeak.Wait();

        var taskWeakPut = Task.Run(async () =>
        {
            await helpersWeak.PutStringBodyOnly(
                $"/api/meta/v1/modules/Tenant/viewModels/FinancialSearch/versions/1/activate", null
            );
        });
        taskWeakPut.Wait();
    }

    private static Dictionary<string, string?>? BayanStatementRequest()
    {
        var bayanStatementRequest = new BayanStatementRequest();
        var helpersBayanStatementRequest = new TypedWrapper<BayanStatementRequest>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_USER") ?? "admin",
            Environment.GetEnvironmentVariable("CREDITLENS_HTTP_PASSWORD") ?? "admin");

        var taskTypedBayanStatementRequest = Task.Run(async () =>
        {
            await helpersBayanStatementRequest.BuildModelAndViews(bayanStatementRequest);
        });
        taskTypedBayanStatementRequest.Wait();
        return helpersBayanStatementRequest.Headers;
    }

    private static void CreateBayanStatement(Dictionary<string, string?>? headers)
    {
        var bayanStatement = new BayanStatement();
        var helpersTypedBayanStatement = new TypedWrapper<BayanStatement>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            headers);

        var taskTypedBayanStatement = Task.Run(async () =>
        {
            await helpersTypedBayanStatement.BuildModelAndViews(bayanStatement);
        });
        taskTypedBayanStatement.Wait();
    }

    private static void CreateBayanSettings(Dictionary<string, string?>? headers)
    {
        var bayanSettings = new BayanSettings();
        var helpersTypedBayanStatement = new TypedWrapper<BayanSettings>(
            new Uri(Environment.GetEnvironmentVariable("CREDITLENS_HTTP_ENDPOINT") ?? "http://localhost"),
            headers);

        var taskTypedBayanStatement = Task.Run(async () =>
        {
            await helpersTypedBayanStatement.BuildModelAndViews(bayanSettings);
        });
        taskTypedBayanStatement.Wait();
    }

    public override void Down()
    {
    }
}